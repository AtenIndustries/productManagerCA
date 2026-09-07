using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.CommonLib.Interceptors;

public class UniqueConstraintInterceptor<TEntity, VException>(string fieldIdName, string uniqueFieldName) : SaveChangesInterceptor
where TEntity : class
where VException : Exception, new()

{
    private readonly string _fieldIdName = fieldIdName;
    private readonly string _uniqueFieldName = uniqueFieldName;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);

        var unqPropertyInfo = typeof(TEntity).GetProperty(_uniqueFieldName) ?? throw new ArgumentException($"Property '{_uniqueFieldName}' does not exist on {typeof(TEntity).Name}.");
        var idPropertyInfo = typeof(TEntity).GetProperty(_fieldIdName) ?? throw new ArgumentException($"Property '{_fieldIdName}' does not exist on {typeof(TEntity).Name}.");

        //Gets the names that are being inserted or updated
        var values = context.ChangeTracker.Entries<TEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => (unqPropertyInfo.GetValue(e.Entity), idPropertyInfo.GetValue(e.Entity)))
            .Where(v => v.Item1 != null && v.Item2 != null)
            .ToList();

        foreach (var value in values)
        {
            //Get existing value in DB
            var existsInDB = context.Set<TEntity>().Any(p =>
                    EF.Property<object>(p, _uniqueFieldName).Equals(value.Item1)
                    && !EF.Property<object>(p, _fieldIdName).Equals(value.Item2) //Ignores the ones with same id
                    );
            //Gets existing from current update or create operation 
            var existsOnSameBatch = values.Count(v => v.Equals(value)) > 1;

            if (existsOnSameBatch || existsInDB)
            {
                throw new VException();
            }

        }

        return base.SavingChangesAsync(eventData, result, ct);
    }
}
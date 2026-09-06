using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProductManager.DAL.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductManager.BAL.Exceptions;
using System.Reflection;


namespace ProductManager.BAL.Tests.Interceptors;
 
public class UniqueConstraintInterceptor<T>(string fieldName) : SaveChangesInterceptor where T : class
{
    private string FieldName { get; set; } = fieldName;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);

        var propertyInfo = typeof(T).GetProperty(FieldName) ?? throw new ArgumentException($"Property '{FieldName}' does not exist on {typeof(T).Name}.");

        //Gets the names that are being inserted or updated
        var values = context.ChangeTracker.Entries<T>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => propertyInfo.GetValue(e.Entity))
            .Where(v => v != null)
            .ToList();

        foreach (var value in values)
        { 
            //Get existing value in DB
            var existsInDB = context.Set<T>().Any(p => EF.Property<object>(p,FieldName).Equals(value));
            //Gets existing from current update or create operation
#pragma warning disable CS8602 // Nulls already filtered above
            var existsOnSameBatch = values.Count(v => v.Equals(value)) > 1;
#pragma warning restore CS8602 //  Nulls already filtered above

            if (existsOnSameBatch || existsInDB){
                throw new DuplicateProductException();
            }

        }

        return base.SavingChangesAsync(eventData, result, ct);
    } 
}


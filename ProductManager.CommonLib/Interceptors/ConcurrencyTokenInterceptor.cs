using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.CommonLib.Interceptors;
/// <summary>
/// [Unit tests exclusive] Makes sure that ConcurrencyTokens are properly filled
/// </summary>
/// <typeparam name="TEntity">Database entity</typeparam>
/// <param name="fieldName">Meta name of the field to add concurrency token</param>
public class ConcurrencyTokenInterceptor<TEntity>(string fieldName) : SaveChangesInterceptor
where TEntity : class
{
    private string FieldName { get; set; } = fieldName;
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);

        // Gets the modified entities
        var entries = context.ChangeTracker.Entries<TEntity>().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            //Gets the the concurrency token property and generates a token
            var property = entry.Property(FieldName);
            property?.CurrentValue = Guid.NewGuid().ToByteArray();
        }
        return base.SavingChangesAsync(eventData, result, ct);
    }

}
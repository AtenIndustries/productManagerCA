using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.API.Tests.Interceptors;
// This interceptor makes sure that ConcurrencyTokens are properly filled
public class ConcurrencyTokenInterceptor(string fieldName) : SaveChangesInterceptor
{
    private string FieldName { get; set; } = fieldName;
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);

        // Gets the modified entities
        var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            //Gets the the concurrency token property and generates a token
            var property = entry.Property(FieldName);
            property?.CurrentValue = Guid.NewGuid().ToByteArray();
        }
        return base.SavingChangesAsync(eventData, result, ct);
    }

}
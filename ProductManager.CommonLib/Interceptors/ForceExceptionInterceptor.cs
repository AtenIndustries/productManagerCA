using Microsoft.EntityFrameworkCore.Diagnostics;
namespace ProductManager.CommonLib.Interceptors;
// This interceptor makes sure that ConcurrencyTokens are properly filled
public class ForceExceptionInterceptor<T>() : SaveChangesInterceptor
    where T : Exception, new()
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        throw new T();
    }

}
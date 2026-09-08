using Microsoft.EntityFrameworkCore.Diagnostics;
namespace ProductManager.CommonLib.Interceptors;


/// <summary>
/// [Unit tests exclusive] Simulates an exception from Database
/// </summary>
/// <typeparam name="T">Exception type</typeparam>
public class ForceExceptionInterceptor<T>() : SaveChangesInterceptor
    where T : Exception, new()
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken ct = default)
    {
        throw new T();
    }

}
using Microsoft.Extensions.DependencyInjection;
using ExamAI.Shared.Middleware;
namespace ExamAI.Shared.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<RequestLoggingMiddleware>();
        return services;
    }
}

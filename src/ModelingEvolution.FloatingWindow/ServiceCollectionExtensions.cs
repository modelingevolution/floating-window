using Microsoft.Extensions.DependencyInjection;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Extension methods for registering FloatingWindow services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FloatingWindow service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFloatingWindow(this IServiceCollection services)
    {
        services.AddScoped<FloatingWindowService>();
        services.AddScoped<IFloatingWindowService>(sp => sp.GetRequiredService<FloatingWindowService>());
        return services;
    }
}

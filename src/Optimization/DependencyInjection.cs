using Microsoft.Extensions.DependencyInjection;

namespace Optimization;

public static class DependencyInjection
{
    public static IServiceCollection AddOptimization(this IServiceCollection services)
    {
        // Register the concrete planning solver here when the planning contract is defined.
        // Keep solver-specific packages (for example OR-Tools) inside this project.
        return services;
    }
}

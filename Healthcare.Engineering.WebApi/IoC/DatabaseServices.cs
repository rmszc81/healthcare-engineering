using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Engineering.WebApi.IoC;

public static class DatabaseServices
{
    public static void AddDatabaseServices(this IServiceCollection services)
    {
        services.AddScoped<Healthcare.Engineering.Database.Model.Seeder>();
    }
}
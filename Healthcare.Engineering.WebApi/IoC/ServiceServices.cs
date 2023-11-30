using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Engineering.WebApi.IoC;

public static class ServiceServices
{
    public static void AddServiceServices(this IServiceCollection services)
    {
        services.AddScoped<Healthcare.Engineering.Services.ChaosGenerator>();
        
        services.AddTransient<Healthcare.Engineering.Services.Interfaces.IDocumentService, Healthcare.Engineering.Services.DocumentService>();
        services.AddTransient<Healthcare.Engineering.Services.Interfaces.IEmailService, Healthcare.Engineering.Services.EmailService>();
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Healthcare.Engineering.WebApi.IoC;

public static class ConfigurationService
{
    public static void AddConfigurationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Healthcare.Engineering.DataObject.Settings.Authentication))
            .Get<Healthcare.Engineering.DataObject.Settings.Authentication>()!);

        builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Healthcare.Engineering.DataObject.Settings.Database))
            .Get<Healthcare.Engineering.DataObject.Settings.Database>()!);
        
        builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(Healthcare.Engineering.DataObject.Settings.RetryPolicy))
            .Get<Healthcare.Engineering.DataObject.Settings.RetryPolicy>()!);
    }
}
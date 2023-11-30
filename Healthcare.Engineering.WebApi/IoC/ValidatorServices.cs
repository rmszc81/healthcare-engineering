using Microsoft.Extensions.DependencyInjection;

using FluentValidation;

namespace Healthcare.Engineering.WebApi.IoC;

public static class ValidatorServices
{
    public static void AddValidatorServices(this IServiceCollection services)
    {
        services.AddScoped<Healthcare.Engineering.Validator.ValidatorSupport>();
        
        services.AddScoped<IValidator<Healthcare.Engineering.DataObject.Data.CustomerDto>, Healthcare.Engineering.Validator.CustomerCreateValidator>();
        services.AddScoped<IValidator<Healthcare.Engineering.DataObject.Data.CustomerDto>, Healthcare.Engineering.Validator.CustomerUpdateValidator>();
        services.AddScoped<IValidator<Healthcare.Engineering.DataObject.Data.CustomerDto>, Healthcare.Engineering.Validator.CustomerDeleteValidator>();
        
        services.AddValidatorsFromAssemblyContaining<Healthcare.Engineering.Validator.CustomerCreateValidator>();
    }
}
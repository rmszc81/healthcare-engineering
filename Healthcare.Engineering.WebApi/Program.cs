using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Healthcare.Engineering.WebApi;

using Filters;
using IoC;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(a => a.Console())
            .WriteTo.Async(a => a.File(GetLogPath(), rollingInterval: RollingInterval.Day))
            .CreateLogger();

        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });
        
        Log.Information("Loading configuration services.");
        builder.AddConfigurationServices();
        
        Log.Information("Loading automapper services.");
        builder.Services.AddAutoMapper(x =>
        {
            x.AllowNullCollections = true;
            x.AllowNullDestinationValues = true;
        }, AppDomain.CurrentDomain.GetAssemblies());
        
        Log.Information("Loading database settings.");
        var databaseSettings = builder.Configuration.GetSection("Database").Get<Healthcare.Engineering.DataObject.Settings.Database>();
        if (string.IsNullOrEmpty(databaseSettings!.FilePath))
            Log.Error("Database file path is empty!");
        else
        {
            builder.Services.AddDbContext<Healthcare.Engineering.Database.Model.Context>(options =>
                options.UseSqlite($"Data Source={databaseSettings.FilePath};",
                    migrations => migrations.MigrationsAssembly("Healthcare.Engineering.Migrations")));
        }
        
        Log.Information("Injecting database services.");
        builder.Services.AddDatabaseServices();

        Log.Information("Injecting validation services.");
        builder.Services.AddValidatorServices();
        
        Log.Information("Injecting service services.");
        builder.Services.AddServiceServices();

        Log.Information("Configuring application.");
        builder.Services
            .AddControllers(options => options.Filters.Add(new GlobalExceptionFilter()))
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(s =>
        {
            s.OperationFilter<Swagger.CustomHeaderSwaggerAttribute>();
        });
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("http://localhost:5194")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .Build();
            });
        });

        Log.Information("Building application configuration.");
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        Log.Information("Configuring forward header options.");
        app.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
        
        // in case there's proxies in the middle of the way //
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto
        });

        Log.Information("Building middleware configuration.");
        app.Use(async (context, next) =>
        {
            var headerApiKey = context.Request.Headers["ApiKey"].ToString();
            if (string.IsNullOrEmpty(headerApiKey))
            {
                Log.Error("The header 'ApiKey' was not found in the request.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var storedHeaderApiKey = app.Services.GetService<Healthcare.Engineering.DataObject.Settings.Authentication>();
            if (!storedHeaderApiKey!.ApiKey!.Equals(headerApiKey, StringComparison.InvariantCulture))
            {
                Log.Error("The header 'ApiKey' does not match the configured value.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next();
        });
        
        Log.Information("Building database.");
        using var scope = app.Services.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<Healthcare.Engineering.Database.Model.Context>();
        await databaseContext.Database.MigrateAsync();
        
        Log.Information("Seeding database data.");
        var databaseSeeder = scope.ServiceProvider.GetRequiredService<Healthcare.Engineering.Database.Model.Seeder>();
        await databaseSeeder.Seed();

        Log.Information("Initialization complete; starting the application.");
        await app.RunAsync();

    }

    private static string GetLogPath()
    {
        const string logFilename = "Healthcare-engineering.log";
        var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "log");

        if (!Directory.Exists(logPath))
            Directory.CreateDirectory(logPath);

        return Path.Combine(logPath, logFilename);
    }
}
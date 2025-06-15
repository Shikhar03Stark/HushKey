using FluentMigrator.Runner;
using HushKeyCore.HushKeyLogger;
using HushKeyCore.Logger;
using HushKeyService.Mapper;
using HushKeyService.Service;
using HuskKeyInfra.Database;
using HuskKeyInfra.Database.Repository;
using HuskKeyInfra.Database.Repository.Postgres;
using HuskKeyInfra.Migrations;
using Npgsql;
using System.Data;

namespace HushKeyApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder.Services);
        ConfigureRepositories(builder.Services);
        ConfigureDatabase(builder);

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        RunMigrations(app);

        app.Run();
    }

    private static void RunMigrations(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register your services here
        services.AddScoped<IHushKeyLogger, ConsoleHushKeyLogger>();
        services.AddAutoMapper(typeof(EncryptedSecretProfile).Assembly);
        services.AddScoped<SecretService>();

    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        // Register repositories with the DI container
        services.AddScoped<IEncryptedSecretRepository, EncryptedSecretRepository>();
        services.AddScoped<IEncryptedSecretStatRepository, EncryptedSecretStatRepository>();
    }

    private static string? GetConnectionStringFromEnv()
    {
        // Use environment variables if connection string is not set
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var username = Environment.GetEnvironmentVariable("DB_USERNAME");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var databaseName = Environment.GetEnvironmentVariable("DB_NAME");
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"; // Default PostgreSQL port

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(databaseName))
        {
            return null;
        }
        var configuration = new DatabaseConfiguration(host, username, password, databaseName);
        return configuration.ConnectionString;
    }

    private static string? GetConnectionStringFromConfiguration(WebApplicationBuilder builder)
    {
        // Try to get connection string from configuration
        var host = builder.Configuration["DatabaseConfiguration:Host"];
        var username = builder.Configuration["DatabaseConfiguration:Username"];
        var password = builder.Configuration["DatabaseConfiguration:Password"];
        var databaseName = builder.Configuration["DatabaseConfiguration:DatabaseName"];
        var port = builder.Configuration["DatabaseConfiguration:Port"] ?? "5432"; // Default PostgreSQL port
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(databaseName))
        {
            return null;
        }
        var configuration = new DatabaseConfiguration(host, username, password, databaseName, int.Parse(port));
        return configuration.ConnectionString;
    }

    private static string GetConnectionString(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Try to get connection string from environment variables
            connectionString = GetConnectionStringFromEnv();
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Try to get connection string from configuration
            connectionString = GetConnectionStringFromConfiguration(builder);
        }

        return connectionString ?? throw new InvalidOperationException("No valid connection string found. Please set the connection string in appsettings.json, environment variables");
    }

    private static void ConfigureDatabase(WebApplicationBuilder builder) 
    {
        var connectionString = GetConnectionString(builder);
        builder.Services.AddSingleton(new DatabaseConfiguration(connectionString));
        builder.Services.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Migrate0InitDatabase).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));
    }
}

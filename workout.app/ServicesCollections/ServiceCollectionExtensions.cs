using Microsoft.SemanticKernel.Agents.OpenAI;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Tokens;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.abstractions.Interfaces.Services;
using workout.logic.Context;
using workout.logic.Options;
using workout.logic.Repositories;
using workout.logic.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Azure.AI.OpenAI;
using Azure;

namespace workout.app.ServicesCollections;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<Tools>();
        services.AddScoped<IModelService, ModelService>();
        services.AddChatClient(new OllamaChatClient("http://localhost:11434", "qwen3:1.7b")).UseFunctionInvocation().Build();
        return services;
    }

    public static IServiceCollection AddAgent(this IServiceCollection services)
    {
        services.TryAddSingleton<AzureOpenAIClient>(sp =>
        {
            var settings = sp.GetRequiredService<Settings>().openAISettings;
            return OpenAIAssistantAgent.CreateAzureOpenAIClient(new AzureKeyCredential(settings.ApiKey), new Uri(settings.BaseUrl));
        });
        services.AddSingleton(services =>
                services.GetRequiredService<AzureOpenAIClient>().GetAssistantClient()
        );
        services.AddSingleton(services =>
                services.GetRequiredService<AzureOpenAIClient>().GetVectorStoreClient()
        );
        services.AddSingleton(services =>
                services.GetRequiredService<AzureOpenAIClient>().GetOpenAIFileClient()
        );
        return services;
    }


    public static IServiceCollection AddInfrastucture(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<Settings>();
        string ConnectionString = config.GetConnectionString("database")!;
        services.AddSignalR();
        services.AddDbContext<WorkoutDbContext>(opts => opts.UseAzureSql(ConnectionString));
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddIdentityCore<User>();
        builder = new IdentityBuilder(builder.UserType, builder.Services);
        builder.AddRoles<IdentityRole>();
        builder.AddEntityFrameworkStores<WorkoutDbContext>();
        builder.AddSignInManager<SignInManager<User>>();
        var sp = services.BuildServiceProvider();
        var tokenSettings = sp.GetRequiredService<Settings>().tokenSettings;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
        {
            opts.UseSecurityTokenValidators = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
            };
        });

        services.AddAuthorization();
        services.AddCors(opts =>
        {
            opts.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
                builder.AllowCredentials();

            });
        });

        return services;
    }

}

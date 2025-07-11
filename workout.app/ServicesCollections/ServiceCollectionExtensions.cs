using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Tokens;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;
using workout.abstractions.Interfaces.Services;
using workout.logic.Context;
using workout.logic.Repositories;
using workout.logic.Services;

namespace workout.app.ServicesCollections;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddChatClient(new OllamaChatClient("http://localhost:11434", "qwen3:1.7b")).UseFunctionInvocation().Build();
        return services;
    }


    public static IServiceCollection AddInfrastucture(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("database");
        services.AddSignalR();
        services.AddDbContext<WorkoutDbContext>(opts => opts.UseNpgsql(connectionString));
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddIdentityCore<User>();
        builder = new IdentityBuilder(builder.UserType, builder.Services);
        builder.AddRoles<IdentityRole>();
        builder.AddEntityFrameworkStores<WorkoutDbContext>();
        builder.AddSignInManager<SignInManager<User>>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
        {
            opts.UseSecurityTokenValidators = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Token:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
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

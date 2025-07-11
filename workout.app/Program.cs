using workout.app.ServicesCollections;
using workout.logic.Hubs;
public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddOpenApi();
        builder.Services.AddInfrastucture(builder.Configuration);
        builder.Services.AddSecurity(builder.Configuration);
        builder.Services.AddServices();

        var app = builder.Build();

        app.MapOpenApi();
        app.UseHttpsRedirection();

        app.MapHub<ServerHub>("server");

        app.Run();
    }
}


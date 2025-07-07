using workout.app.ServicesCollections;
public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddOpenApi();
        builder.Services.AddSecurity(builder.Configuration);

        var app = builder.Build();

        app.MapOpenApi();
        app.UseHttpsRedirection();



        app.Run();
    }
}


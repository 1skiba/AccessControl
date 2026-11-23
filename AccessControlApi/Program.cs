using AccessControlApi.Data;
using AccessControlApi.Mqtt;
using AccessControlApi.Services;
using Microsoft.EntityFrameworkCore;

namespace AccessControlApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AccessControlDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("Mqtt"));
        builder.Services.AddScoped<AccessService>();
        builder.Services.AddHostedService<MqttListener>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
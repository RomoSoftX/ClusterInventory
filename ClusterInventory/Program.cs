
namespace ClusterInventory
{
    using ClusterInventory.Settings;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Models;
    using Repositories;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Settings Mongo
            var mongoSection = builder.Configuration.GetSection("MongoDb");
            var mongo = mongoSection.Get<MongoDbSettings>()
                          ?? throw new InvalidOperationException("MongoDb settings missing");
            builder.Services.AddSingleton(mongo);

            // Repositorios
            builder.Services.AddSingleton<IItemRepository, ItemRepository>();
            builder.Services.AddSingleton<IMovementRepository, MovementRepository>();


            builder.Services.AddControllers();

            // CORS (opcional)
            builder.Services.AddCors(opt =>
                opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
            );

            var app = builder.Build();
            app.MapGet("/_endpoints", ([FromServices] IEnumerable<EndpointDataSource> sources) =>
            {
                return Results.Ok(
                    sources.SelectMany(s => s.Endpoints)
                           .Select(e => e.DisplayName)
                           .OrderBy(s => s)
                );
            });

            app.UseCors();
            app.MapGet("/ping", () => Results.Ok("pong"));

            // Mapear controladores
            app.MapControllers();

            app.Run();
        }
    }
}



using APBD7.Repository;
using APBD7.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
        builder.Services.AddSingleton<IWarehouseRepository, WarehouseRepository>();
        // builder.Services.AddScoped<IAnimalService, AnimalService>();
        // builder.Services.AddSingleton<IAnimalRepository,AnimalRepository>();
        
        var app = builder.Build();

        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
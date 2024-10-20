using System.Reflection;
using FinBeatTech.DataProcessor;
using FinBeatTech.DataProcessor.Util;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using NLog.Extensions.Logging;

namespace FinBeatTech;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            builder.Services.AddControllers(); 
            builder.Services.AddControllers()
                .AddNewtonsoftJson();

            ConfigureServices.SetDIConfig(builder.Services);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;  // Логируем все поля
            });
            builder.Logging.AddProvider(new DatabaseLoggerProvider());
            var app = builder.Build();

                
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpLogging();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            logger.Info("App started");
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}

using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // configure Serilog
            ConfigureLogging();

            // add Serilog to host
            builder.Host.UseSerilog();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElastic(configuration, environment))
                .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElastic(IConfigurationRoot configuration, string? environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ELK:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"ELKDemo"
            };
        }
    }
}
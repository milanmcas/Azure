
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;

namespace AzureAppAccess
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            const string ConnectionString = "Endpoint=https://appcs-53504475.azconfig.io;Id=1ltf;Secret=1XxAyOyl8Q1RVfttxOOVgimzHOygl1UhLOzIVCIRssOX8SK8r2DDJQQJ99BHAC8vTIndyv2KAAACAZACThAx";
            
            builder.Services.Configure<MyConfig>(builder.Configuration.GetSection("MyNiceConfig"));
            //builder.Configuration.AddAzureAppConfiguration(options =>
            //{
            //    options.Connect(ConnectionString).ConfigureRefresh((refreshOptions) =>
            //    {
            //        // indicates that all configuration should be refreshed when the given key has changed.
            //        refreshOptions.Register(key: "MyNiceConfig:PageSize", refreshAll: true).SetCacheExpiration(TimeSpan.FromSeconds(10));
            //        //refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(5));
            //    }).UseFeatureFlags();
            //});
            builder.Services.AddAzureAppConfiguration();
            builder.Services.AddFeatureManagement();
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
            // Enable automatic configuration refresh from Azure App Configuration
            //app.UseAzureAppConfiguration();

            app.MapControllers();

            app.Run();
        }
    }
}

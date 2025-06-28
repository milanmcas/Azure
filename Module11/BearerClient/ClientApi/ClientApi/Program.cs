using ClientApi;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Options>(builder.Configuration.Get<Options>()??new Options());
builder.Services.AddSingleton<IConfidentialClientApplication>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    return ConfidentialClientApplicationBuilder.Create(config["AzureAd:ClientId"])
        .WithClientSecret(config["AzureAd:ClientSecret"])
        .WithAuthority(new Uri($"{config["AzureAd:Instance"]}{config["AzureAd:TenantId"]}"))
        .Build();
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITokenService, TokenService>();
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

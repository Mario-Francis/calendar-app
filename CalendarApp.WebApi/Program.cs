using CalendarApp.WebApi.Models;
using CalendarApp.WebApi.Providers;
using CalendarApp.WebApi.Services;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection("AzureAdSettings"));
builder.Services.AddSingleton<IAuthenticationProvider, GraphAuthProvider>();

builder.Services.AddSingleton(sp =>
{
    var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
    return new GraphServiceClient(authProvider);
});
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddControllers();
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
app.MapControllers();

await app.RunAsync();
using WeatherAPI.Application.Interfaces.Repositories;
using WeatherAPI.Application.Interfaces.Services;
using WeatherAPI.Application.Interfaces.Utils;
using WeatherAPI.Application.Services;
using WeatherAPI.Infrastructure.Mappings;
using WeatherAPI.Infrastructure.Mappings.Converter;
using WeatherAPI.Infrastructure.Repositories;
using WeatherAPI.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Config API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});

//Getting allow CORS origins
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Singleton
builder.Services.AddSingleton<ICachingUtil, CachingUtil>();
//Scoped
builder.Services.AddScoped<IParallelRequestProcessor, ParallelRequestProcessor>();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddScoped<IForeCast5Repository, ForeCast5Repository>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
//Trasient
builder.Services.AddTransient<WeatherDataToDayForecastDtoConverter>();

//Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<InfrastructureMappingProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

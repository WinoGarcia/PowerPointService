using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using PowerPointService.Services;
using PowerPointService.Types;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddSwaggerGen(op => { op.SwaggerDoc("slides", new OpenApiInfo { Title = "PowerPoint Service", Version = "slides" }); })
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<SettingOptions>(configuration.GetSection(SettingOptions.Settings));
var options = configuration.GetSection(SettingOptions.Settings).Get<SettingOptions>();

builder.Services.AddSingleton<IFFMpegService, FFMpegService>();
builder.Services.AddScoped<IPowerPointParser, PowerPointParser>();
builder.Services.AddScoped<IPowerPointService, PowerPointService.Services.PowerPointService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/slides/swagger.json", "slides");
    c.RoutePrefix = string.Empty;
    c.DefaultModelsExpandDepth(-1);
});

app.UseCors(
    b => b
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();
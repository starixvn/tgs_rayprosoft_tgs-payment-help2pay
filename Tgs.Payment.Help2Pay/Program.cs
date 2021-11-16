using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddHelp2Pay((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        config.GetSection("Help2Pay").Bind(options);

        var jsonOptions = sp.GetRequiredService<IOptions<JsonOptions>>().Value;
        options.SystemSerializerOptions = jsonOptions.JsonSerializerOptions;
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Logging.AddTgsLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.MapGet("/", async context => await context.Response.WriteAsync("Hello World"));

app.Run();
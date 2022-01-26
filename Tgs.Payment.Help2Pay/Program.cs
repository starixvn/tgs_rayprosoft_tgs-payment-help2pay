using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddHelp2Pay((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        config.GetSection("Help2Pay").Bind(options);

        var jsonOptions = sp.GetRequiredService<IOptions<JsonOptions>>().Value;
        options.SystemSerializerOptions = jsonOptions.JsonSerializerOptions;
    })
    .UseRedis((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.ConnectionString = config.GetConnectionString("Redis");
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });

builder.Configuration.AddJsonFile("/config/appsettings.json", true);
builder.Logging.AddTgsLog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    });

builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter(builder =>
        {
            builder
                .AddFilter(new FileExtensionFilter("html", "ico", "png", "jpg"))
                .AddFilter(new UrlPathFilter("/swagger"))
                .AddFilter(new UrlPathFilter("/healthz"));
        })
        .AddOtlpExporter();
});

builder.Services.Configure<AspNetCoreInstrumentationOptions>(options =>
{
    options.Filter = httpContext =>
    {
        return !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !new[] { "html", "ico", "png", "jpg" }.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
    };
});

builder.Services.AddProfileViewer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapHealthChecks("/healthz",
    new HealthCheckOptions
    {
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError }
        },
        ResponseWriter = async (
            context,
            report) =>
        {
            var hostName = Dns.GetHostName();
            var hostEntry = await Dns.GetHostEntryAsync(hostName);
            var hostIp = Array.Find(hostEntry.AddressList,
                x => x.AddressFamily == AddressFamily.InterNetwork);
            var response = new
            {
                Data = new
                {
                    Environment.MachineName,
                    HostName = hostName,
                    HostIp = hostIp?.ToString() ?? string.Empty,
                    Environment = app.Environment.EnvironmentName,
                    OsVersion = $"{Environment.OSVersion}",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                    ProcessCount = Environment.ProcessorCount
                },
                Result = new
                {
                    Status = report.Status.ToString(),
                    TotalDuration = report.TotalDuration.TotalSeconds,
                    DependencyServices = report.Entries.Select(srv => new
                    {
                        srv.Key,
                        Duration = srv.Value.Duration.TotalSeconds,
                        srv.Value.Description,
                        srv.Value.Data,
                        Exception = srv.Value.Exception?.Message,
                        Status = srv.Value.Status.ToString(),
                        Tags = srv.Value.Tags.Any() ? string.Join(',', srv.Value.Tags) : string.Empty
                    }).ToArray()
                }
            };
            var json = JsonSerializer.Serialize(response,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    });

app.MapProfileViewer();

app.Run();

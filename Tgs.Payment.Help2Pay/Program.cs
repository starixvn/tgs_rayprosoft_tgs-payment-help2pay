using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.Help2Pay.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddHttpClientLoggingHandler()
    .AddHelp2Pay(
        (options, sp) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            config.GetSection("Help2Pay").Bind(options);

            var jsonOptions = sp.GetRequiredService<IOptions<JsonOptions>>().Value;
            options.SystemSerializerOptions = jsonOptions.JsonSerializerOptions;
        },
        sp => sp.ResolveLoggingHandler<Help2PayOptions>())
    .UseRedis((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.ConnectionString = config.GetConnectionString("Redis");
    });
builder
    .Services
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
    .ConfigQueueOptions((options, _) =>
        options.Url = builder.Configuration.GetConnectionString("Nats"))
    .AddProcessor(ProcessorTypes.DepositProcessorType, "tgs.payment.help2pay.deposit", "tgs.payment.help2pay.deposit")
    .AddProcessor(ProcessorTypes.WithdrawProcessorType, "tgs.payment.help2pay.withdraw", "tgs.payment.help2pay.withdraw"))
.AddNatsGlobPatternExchange("*");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });

builder.Services
    .AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.MediaTypeOptions.AddText("application/x-www-form-urlencoded");
        logging.MediaTypeOptions.AddText("text/html");
    });

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, config) =>
    {
        var nodeIp = config.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        config.GetSection("Opentelemetry:Tracing").Bind(options);
    });

builder.Services
    .AddOpenTelemetryTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
                       || !httpContext.Request.Path.StartsWithSegments("/swagger")
                       && !httpContext.Request.Path.StartsWithSegments("/healthz")
                       && !httpContext.Request.Path.StartsWithSegments("/metrics")
                       && !new[] { "html", "ico", "png", "jpg" }.Any(
                           ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
    .AddProfileViewer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpLogging();

app.MapControllers();
app.MapHealthChecks("/healthz");
app.MapProfileViewer();
app.UseSwaggerUI();
app.MapSwagger();

app.Use(async (context, next) =>
{
    await next().ConfigureAwait(false);

    var logger = context.RequestServices.GetService<ILoggerFactory>()?
        .CreateLogger("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware");

    if (logger?.IsEnabled(LogLevel.Information) == true)
    {
        using var stream = new StreamReader(context.Request.Body);
        _ = await stream.ReadToEndAsync().ConfigureAwait(false);
    }
});

app.Run();

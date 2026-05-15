using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Diagnostics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var observability = builder.Configuration.GetSection("Observability").Get<ObservabilityOptions>() ?? new ObservabilityOptions();
observability = observability with
{
    ServiceName = FirstNonEmpty(
        Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME"),
        observability.ServiceName,
        "observability-kit-example-api"),
    OtlpEndpoint = FirstNonEmpty(
        Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"),
        observability.OtlpEndpoint,
        "http://localhost:4317"),
    Environment = FirstNonEmpty(
        observability.Environment,
        "dev")
};

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(
        serviceName: observability.ServiceName,
        serviceNamespace: observability.ServiceNamespace,
        serviceVersion: observability.ServiceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = observability.Environment,
        ["telemetry.platform"] = "observability-kit"
    });

builder.Services.AddSingleton(observability);
builder.Services.AddHttpClient("dependency-client", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton(new ExampleTelemetry());
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.Clear().AddService(
        observability.ServiceName,
        serviceNamespace: observability.ServiceNamespace,
        serviceVersion: observability.ServiceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = observability.Environment,
            ["telemetry.platform"] = "observability-kit"
        }))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter(ExampleTelemetry.MeterName)
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(observability.OtlpEndpoint);
            });
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(ExampleTelemetry.ActivitySourceName)
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(observability.OtlpEndpoint);
            });
    });

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ ";
});
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.SetResourceBuilder(resourceBuilder);
    options.AddOtlpExporter(exporter =>
    {
        exporter.Endpoint = new Uri(observability.OtlpEndpoint);
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        if (feature?.Error is not null)
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("UnhandledException");
            logger.LogError(feature.Error, "Unhandled request failure for {Path}", context.Request.Path);
        }

        await context.Response.WriteAsJsonAsync(new
        {
            error = "controlled_failure",
            path = context.Request.Path.Value
        });
    });
});

app.MapGet("/health", (ObservabilityOptions options) =>
{
    return Results.Ok(new
    {
        status = "healthy",
        service = options.ServiceName,
        environment = options.Environment
    });
});

app.MapGet("/api/work", (ExampleTelemetry telemetry, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("WorkEndpoint");

    using var activity = telemetry.ActivitySource.StartActivity("work execute", ActivityKind.Internal);
    activity?.SetTag("operation.name", "work");
    activity?.SetTag("operation.type", "http");
    activity?.SetTag("result", "success");

    var startedAt = Stopwatch.GetTimestamp();
    telemetry.RequestsProcessed.Add(1, new("operation", "work"), new("result", "success"));
    logger.LogInformation("Work request completed successfully");
    telemetry.RequestDuration.Record(GetElapsedMilliseconds(startedAt), new("operation", "work"), new("result", "success"));

    return Results.Ok(new
    {
        result = "ok",
        operation = "work"
    });
});

app.MapGet("/api/fail", (ExampleTelemetry telemetry, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("FailEndpoint");

    using var activity = telemetry.ActivitySource.StartActivity("fail execute", ActivityKind.Internal);
    activity?.SetTag("operation.name", "fail");
    activity?.SetTag("operation.type", "http");
    activity?.SetTag("result", "failure");

    telemetry.RequestsFailed.Add(1, new("operation", "fail"), new("result", "failure"), new("error.type", "InvalidOperationException"));
    logger.LogWarning("Controlled failure endpoint invoked");

    throw new InvalidOperationException("Controlled failure for observability validation");
});

app.MapGet("/api/slow", async (ExampleTelemetry telemetry, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("SlowEndpoint");
    var delay = Random.Shared.Next(500, 3000);

    using var activity = telemetry.ActivitySource.StartActivity("slow execute", ActivityKind.Internal);
    activity?.SetTag("operation.name", "slow");
    activity?.SetTag("operation.type", "http");
    activity?.SetTag("delay.ms", delay);

    var startedAt = Stopwatch.GetTimestamp();
    await Task.Delay(delay, cancellationToken);
    telemetry.RequestsProcessed.Add(1, new("operation", "slow"), new("result", "success"));
    telemetry.RequestDuration.Record(GetElapsedMilliseconds(startedAt), new("operation", "slow"), new("result", "success"));
    logger.LogInformation("Slow endpoint completed after {DelayMs}ms", delay);

    return Results.Ok(new
    {
        result = "ok",
        operation = "slow",
        delayMs = delay
    });
});

app.MapGet("/api/dependency", async (IHttpClientFactory httpClientFactory, ExampleTelemetry telemetry, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("DependencyEndpoint");

    using var activity = telemetry.ActivitySource.StartActivity("dependency call", ActivityKind.Client);
    activity?.SetTag("operation.name", "dependency");
    activity?.SetTag("operation.type", "http");
    activity?.SetTag("dependency.name", "httpbin");
    activity?.SetTag("dependency.type", "external-api");

    var startedAt = Stopwatch.GetTimestamp();
    var client = httpClientFactory.CreateClient("dependency-client");
    var response = await client.GetAsync("https://httpbin.org/status/200", cancellationToken);
    response.EnsureSuccessStatusCode();

    telemetry.ExternalDependencyDuration.Record(
        GetElapsedMilliseconds(startedAt),
        new("dependency.name", "httpbin"),
        new("dependency.type", "external-api"),
        new("result", "success"));

    telemetry.RequestsProcessed.Add(1, new("operation", "dependency"), new("result", "success"));
    telemetry.RequestDuration.Record(GetElapsedMilliseconds(startedAt), new("operation", "dependency"), new("result", "success"));
    logger.LogInformation("External dependency call completed with status code {StatusCode}", (int)response.StatusCode);

    return Results.Ok(new
    {
        result = "ok",
        operation = "dependency",
        upstreamStatusCode = (int)response.StatusCode
    });
});

app.MapPost("/api/jobs", async (ExampleTelemetry telemetry, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("JobsEndpoint");

    using var activity = telemetry.ActivitySource.StartActivity("job process sample", ActivityKind.Internal);
    activity?.SetTag("operation.name", "jobs");
    activity?.SetTag("operation.type", "job");
    activity?.SetTag("job.type", "sample");

    Interlocked.Increment(ref telemetry.PendingQueueItems);
    var startedAt = Stopwatch.GetTimestamp();

    try
    {
        await Task.Delay(Random.Shared.Next(150, 1200), cancellationToken);
        telemetry.JobsProcessed.Add(1, new("job.type", "sample"), new("result", "success"));
        logger.LogInformation("Background job processed successfully");

        return Results.Accepted("/api/jobs", new
        {
            result = "accepted",
            jobType = "sample"
        });
    }
    catch (Exception ex)
    {
        telemetry.JobsFailed.Add(1, new("job.type", "sample"), new("result", "failure"), new("error.type", ex.GetType().Name));
        logger.LogError(ex, "Background job simulation failed");
        throw;
    }
    finally
    {
        Interlocked.Decrement(ref telemetry.PendingQueueItems);
        telemetry.JobDuration.Record(
            GetElapsedMilliseconds(startedAt),
            new TagList
            {
                { "job.type", "sample" }
            });
    }
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    app.Logger.LogInformation("Service started");
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    app.Logger.LogInformation("Service stopping");
});

app.Run();

static double GetElapsedMilliseconds(long startedAt)
{
    return Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;
}

static string FirstNonEmpty(params string?[] values)
{
    foreach (var value in values)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
    }

    return string.Empty;
}

internal sealed record ObservabilityOptions
{
    public string ServiceName { get; init; } = "observability-kit-example-api";
    public string ServiceNamespace { get; init; } = "a2g";
    public string ServiceVersion { get; init; } = "1.0.0";
    public string Environment { get; init; } = "dev";
    public string OtlpEndpoint { get; init; } = "http://localhost:4317";
}

internal sealed class ExampleTelemetry
{
    public const string MeterName = "ObservabilityKit.Example.Api";
    public const string ActivitySourceName = "ObservabilityKit.Example.Api";

    private readonly Meter _meter = new(MeterName, "1.0.0");

    public ExampleTelemetry()
    {
        RequestsProcessed = _meter.CreateCounter<long>("app.requests.processed");
        RequestsFailed = _meter.CreateCounter<long>("app.requests.failed");
        RequestDuration = _meter.CreateHistogram<double>("app.request.duration", unit: "ms");
        JobsProcessed = _meter.CreateCounter<long>("app.jobs.processed");
        JobsFailed = _meter.CreateCounter<long>("app.jobs.failed");
        JobDuration = _meter.CreateHistogram<double>("app.jobs.duration", unit: "ms");
        ExternalDependencyDuration = _meter.CreateHistogram<double>("app.external_dependency.duration", unit: "ms");
        _meter.CreateObservableGauge("app.queue.items.pending", () => PendingQueueItems);
    }

    public ActivitySource ActivitySource { get; } = new(ActivitySourceName, "1.0.0");
    public Counter<long> RequestsProcessed { get; }
    public Counter<long> RequestsFailed { get; }
    public Histogram<double> RequestDuration { get; }
    public Counter<long> JobsProcessed { get; }
    public Counter<long> JobsFailed { get; }
    public Histogram<double> JobDuration { get; }
    public Histogram<double> ExternalDependencyDuration { get; }
    public long PendingQueueItems;
}

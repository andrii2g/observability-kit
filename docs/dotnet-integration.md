# Dotnet Integration

## Example Project

Reference implementation:

- [examples/dotnet-minimal-api/Program.cs](C:/github/a2g.name/observability-kit/examples/dotnet-minimal-api/Program.cs)

## Runtime Endpoints

- Host execution: `http://localhost:4317`
- Compose execution: `http://otel-collector:4317`

## Required Instrumentation

- ASP.NET Core request traces
- HttpClient traces
- runtime metrics
- process metrics
- custom `Meter`
- custom `ActivitySource`
- OTLP exporter for metrics, traces, and logs

## Custom Instruments

The example defines:

- `app.requests.processed`
- `app.requests.failed`
- `app.request.duration`
- `app.jobs.processed`
- `app.jobs.failed`
- `app.jobs.duration`
- `app.queue.items.pending`
- `app.external_dependency.duration`

## Example Endpoints

- `GET /health`
- `GET /api/work`
- `GET /api/fail`
- `GET /api/slow`
- `GET /api/dependency`
- `POST /api/jobs`

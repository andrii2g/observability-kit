# Dotnet Minimal API Example

This example is the Phase 4 sample service for `observability-kit`.

## Runtime Endpoints

Host execution uses:

```text
http://localhost:4317
```

Docker Compose execution uses:

```text
http://otel-collector:4317
```

## Example Endpoints

- `GET /health`
- `GET /api/work`
- `GET /api/fail`
- `GET /api/slow`
- `GET /api/dependency`
- `POST /api/jobs`

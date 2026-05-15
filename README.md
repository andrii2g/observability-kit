# observability-kit

Reusable OpenTelemetry-first observability stack for local development, small services, internal tools, and production-like environments.

## Status

This repository is being implemented in review checkpoints. Phase 1 provides the project skeleton only.

## Planned Stack

- OpenTelemetry Collector as the only application telemetry ingress
- Prometheus for metrics
- Tempo for traces
- Grafana for dashboards
- Alertmanager for alerts
- Optional Elasticsearch and Kibana profile for logs

## Supported Startup Paths

Core mode:

```bash
docker compose up -d
```

ELK mode:

```bash
./scripts/up-elk.sh
```

## Documentation

- [Quickstart](docs/quickstart.md)
- [Architecture](docs/architecture.md)
- [Service Onboarding](docs/service-onboarding.md)
- [Dotnet Integration](docs/dotnet-integration.md)
- [Metrics Catalog](docs/metrics-catalog.md)
- [Logging Catalog](docs/logging-catalog.md)
- [Tracing Catalog](docs/tracing-catalog.md)
- [Alerts Catalog](docs/alerts-catalog.md)
- [ELK Mode](docs/elk-mode.md)
- [Troubleshooting](docs/troubleshooting.md)

## Repository Layout

Phase 1 creates the skeleton for:

- `docs/`
- `scripts/`
- `otel-collector/`
- `prometheus/`
- `alertmanager/`
- `grafana/`
- `tempo/`
- `elasticsearch/`
- `kibana/`
- `examples/`

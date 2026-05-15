# observability-kit

Reusable OpenTelemetry-first observability stack for local development, small services, internal tools, and production-like environments.

## Why This Repository Exists

Applications send telemetry only to OpenTelemetry Collector. They do not depend directly on Prometheus, Tempo, Elasticsearch, Kibana, or Grafana. That keeps service instrumentation stable while allowing the platform to switch storage and visualization at the infrastructure layer.

## Stack

- OpenTelemetry Collector as the only telemetry ingress
- Prometheus for metrics
- Tempo for traces
- Grafana for dashboards
- Alertmanager for alerts
- Optional Elasticsearch and Kibana profile for logs

## Supported Startup Paths

Core mode:

```bash
cp .env.example .env
docker compose up -d
```

ELK mode:

```bash
./scripts/up-elk.sh
```

Raw ELK equivalent:

```bash
OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml docker compose --profile elk up -d
```

## URLs

- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090
- Alertmanager: http://localhost:9093
- Tempo: http://localhost:3200
- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601

## Service Onboarding Summary

1. Choose `service.name`, `service.namespace`, and `deployment.environment`.
2. Send OTLP telemetry to the collector.
3. Use `http://localhost:4317` for host-running services.
4. Use `http://otel-collector:4317` for Compose-running services.
5. Verify metrics in Grafana, traces in Grafana/Tempo, and logs in Kibana when ELK is enabled.

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

## Architecture

```text
Applications
   |
   | OTLP gRPC / HTTP
   v
OpenTelemetry Collector
   |-- metrics --> Prometheus --> Grafana
   |-- traces  --> Tempo      --> Grafana
   `-- logs    --> debug exporter (default)
                --> Elasticsearch --> Kibana (ELK mode)
```

## Screenshots

Screenshots will be added after the stack is run end to end.

## Repository Layout

Key directories:

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

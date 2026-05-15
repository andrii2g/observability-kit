# Architecture

## Core Rule

Applications send telemetry only to OpenTelemetry Collector. Storage and UI backends remain a platform concern.

## Default Mode

```text
Applications
   |
   | OTLP gRPC / HTTP
   v
OpenTelemetry Collector
   |-- metrics --> Prometheus --> Grafana
   |-- traces  --> Tempo      --> Grafana
   `-- logs    --> debug exporter
```

Start command:

```bash
docker compose up -d
```

## ELK Mode

```text
Applications
   |
   | OTLP gRPC / HTTP
   v
OpenTelemetry Collector
   |-- metrics --> Prometheus --> Grafana
   |-- traces  --> Tempo      --> Grafana
   `-- logs    --> Elasticsearch --> Kibana
```

Start command:

```bash
./scripts/up-elk.sh
```

Raw equivalent:

```bash
OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml docker compose --profile elk up -d
```

## Runtime Endpoints

- Host-running services: `http://localhost:4317`
- Compose-running services: `http://otel-collector:4317`

## Collector Config Switch

The collector always mounts both configs:

- `/etc/otelcol/config.default.yaml`
- `/etc/otelcol/config.elk.yaml`

`OTEL_COLLECTOR_CONFIG` chooses which one is active. ELK mode requires both:

1. Compose profile `elk`
2. `OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml`

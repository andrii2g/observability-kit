# Quickstart

## Start Core Stack

```bash
cp .env.example .env
docker compose up -d
docker compose ps
```

Open:

- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090
- Alertmanager: http://localhost:9093
- Tempo: http://localhost:3200

## Start Core Stack With Example Service

```bash
docker compose --profile examples up -d
```

The example API is exposed at:

- Example API: http://localhost:8080

## Start ELK Mode

```bash
./scripts/up-elk.sh
```

Raw equivalent:

```bash
OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml docker compose --profile elk up -d
```

Open:

- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601

## Stop And Reset

```bash
./scripts/down.sh
./scripts/reset-data.sh
```

## Notes

- Applications always send telemetry to OpenTelemetry Collector.
- Host-running services use `http://localhost:4317`.
- Compose-running services use `http://otel-collector:4317`.

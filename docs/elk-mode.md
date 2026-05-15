# ELK Mode

## Purpose

ELK mode is optional. It exists for teams that want Elasticsearch/Kibana log search without changing application instrumentation.

## Supported Command

```bash
./scripts/up-elk.sh
```

Raw equivalent:

```bash
OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml docker compose --profile elk up -d
```

Do not treat bare `docker compose --profile elk up -d` as the supported ELK startup path. That starts ELK services, but it does not switch the collector unless `OTEL_COLLECTOR_CONFIG` is set correctly.

## What Changes In ELK Mode

- application code does not change
- host/container OTLP endpoint rules do not change
- logs are exported to Elasticsearch
- Kibana becomes the main log exploration UI
- Compose also runs an ELK bootstrap step that applies the ILM policy and index template automatically

## Kibana Data View

Use:

```text
otel-logs-*
```

Bootstrap artifacts applied automatically:

- `elasticsearch/ilm-policies/otel-logs-ilm-policy.json`
- `elasticsearch/index-templates/otel-logs-template.json`

## Common Searches

- by service: `service.name: "observability-kit-example-api"`
- by trace: `trace_id: "<trace-id>"`
- by environment: `deployment.environment: "dev"`
- exceptions: `exception.type:*`

## Disabling ELK

Stop the stack and restart core mode:

```bash
./scripts/down.sh
docker compose up -d
```

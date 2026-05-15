#!/usr/bin/env bash
set -euo pipefail

mkdir -p rendered

docker compose config > rendered/observability-kit-compose.rendered.yml
OTEL_COLLECTOR_CONFIG=/etc/otelcol/config.elk.yaml docker compose --profile elk config > rendered/observability-kit-compose-elk.rendered.yml

echo "Compose config is valid for core and ELK modes"

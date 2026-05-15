#!/usr/bin/env bash
set -euo pipefail

export OTEL_COLLECTOR_CONFIG="/etc/otelcol/config.elk.yaml"

docker compose --profile elk up -d

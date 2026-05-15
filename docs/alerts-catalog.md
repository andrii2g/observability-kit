# Alerts Catalog

## Alert Groups

- `service-health`
- `http-api`
- `runtime`
- `collector`
- `host`

## Service Alerts

- `ServiceDown`
- `NoTelemetryReceived`
- `ServiceRestartingFrequently`

## HTTP Alerts

- `HighHttp5xxRate`
- `HighHttp4xxRate`
- `HighP95Latency`
- `NoSuccessfulRequests`

## Runtime Alerts

- `HighCpuUsage`
- `HighMemoryUsage`
- `FrequentGcCollections`
- `HighThreadPoolQueue`
- `HighExceptionRate`

## Collector Alerts

- `OtelCollectorExporterFailures`
- `OtelCollectorDroppedSpans`
- `OtelCollectorDroppedMetricPoints`
- `OtelCollectorDroppedLogs`

## Tuning

All thresholds should be reviewed per service workload and environment. Development defaults are intentionally simple and should not be treated as production policy.

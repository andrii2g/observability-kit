# Service Onboarding

## Checklist

1. Choose `service.name`.
2. Choose `service.namespace`.
3. Set `deployment.environment`.
4. Choose OTLP endpoint for runtime context.
5. Add OpenTelemetry SDK.
6. Enable traces.
7. Enable metrics.
8. Enable logs.
9. Add custom metrics.
10. Add health endpoint.
11. Run local stack.
12. Confirm metrics in Grafana.
13. Confirm traces in Grafana.
14. Confirm logs in Kibana if ELK is enabled.
15. Add service-specific alerts.

## OTLP Endpoint Convention

- Host execution: `http://localhost:4317`
- Compose execution: `http://otel-collector:4317`

## Resource Attributes

At minimum, set:

- `service.name`
- `service.namespace`
- `service.version`
- `deployment.environment`

## Acceptance Signal

A service is onboarded when:

- metrics appear in Grafana through Prometheus
- traces appear in Grafana through Tempo
- logs appear in Kibana when ELK mode is enabled

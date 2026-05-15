# Logging Catalog

## Required Fields

- `timestamp`
- `level`
- `message`
- `service.name`
- `deployment.environment`
- `trace_id`
- `span_id`
- `operation`

## Recommended Fields

- `request.id`
- `correlation.id`
- `error.type`
- `error.code`
- `duration.ms`
- `http.method`
- `http.route`
- `http.status_code`
- `dependency.name`
- `dependency.type`

## Do Not Log

- passwords
- API keys
- bearer tokens
- refresh tokens
- raw cookies
- private keys
- connection strings with credentials
- personal data unless explicitly required and protected

## Level Guidance

- `Trace`: deep diagnostics
- `Debug`: developer diagnostics
- `Information`: normal business/system events
- `Warning`: recoverable abnormal behavior
- `Error`: failed operation
- `Critical`: service-level failure

## Correlation

When logs are emitted inside active spans, they should include trace and span correlation fields so Kibana searches can pivot from logs to traces.

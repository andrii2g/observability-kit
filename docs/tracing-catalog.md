# Tracing Catalog

## Use Traces For

- HTTP requests
- background jobs
- external dependencies
- database calls
- expensive operations
- file processing
- external API calls

## Span Naming

Prefer stable names:

- `HTTP GET /api/work`
- `job process PriceCheck`
- `dependency call httpbin`

Avoid high-cardinality names:

- Bad: `HTTP GET /api/users/123456`
- Good: `HTTP GET /api/users/{id}`

## Required Span Attributes

- `service.name`
- `deployment.environment`
- `operation.name`
- `operation.type`
- `result`
- `error.type`

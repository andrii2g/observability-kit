# Metrics Catalog

## Naming Rules

1. Use lowercase metric names.
2. Use dot-separated namespaces for application metrics.
3. Keep names stable.
4. Avoid high-cardinality labels.
5. Prefer route templates over raw paths.
6. Use histograms for durations.
7. Use counters for totals.
8. Use gauges for current values.

Do not put user IDs, request IDs, email addresses, or raw URLs into labels.

## Standard Resource Labels

- `service.name`
- `service.namespace`
- `service.version`
- `deployment.environment`
- `host.name`
- `container.name`

## HTTP Metrics

- `http.server.request.duration`
- `http.server.request.count`
- `http.server.active_requests`
- `http.client.request.duration`
- `http.client.request.count`

Suggested labels:

- `http.request.method`
- `http.route`
- `http.response.status_code`

## Application Metrics

- `app.requests.processed`
- `app.requests.failed`
- `app.request.duration`
- `app.operation.duration`
- `app.operation.failed`

## Background Job Metrics

- `app.jobs.processed`
- `app.jobs.failed`
- `app.jobs.duration`
- `app.jobs.retry.count`
- `app.jobs.deadletter.count`

## Queue Metrics

- `app.queue.items.pending`
- `app.queue.items.processed`
- `app.queue.items.failed`
- `app.queue.processing.duration`

## Dependency Metrics

- `app.dependency.duration`
- `app.dependency.calls`
- `app.dependency.errors`

## .NET Runtime Metrics

- `process.cpu.time`
- `process.memory.usage`
- `process.runtime.dotnet.gc.collections.count`
- `process.runtime.dotnet.gc.heap.size`
- `process.runtime.dotnet.thread_pool.threads.count`
- `process.runtime.dotnet.exceptions.count`

## Service Examples

- Web API: request rate, latency, error rate
- Worker service: job throughput, queue depth, retry rate
- CLI/tool service: execution duration, exit status, dependency latency

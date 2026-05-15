# Troubleshooting

## Collector Does Not Start

Check:

```bash
docker compose logs otel-collector
docker compose config
```

Common causes:

- invalid YAML
- wrong exporter name
- ELK exporter enabled without Elasticsearch
- unsupported collector config option

## No Metrics In Grafana

Check:

```bash
docker compose logs prometheus
```

Then inspect:

- http://localhost:9090/targets

Common causes:

- collector not exposing Prometheus exporter
- Prometheus scrape target down
- service not sending OTLP metrics
- wrong OTLP endpoint

## No Logs In Kibana

Check:

```bash
docker compose --profile elk ps
docker compose logs otel-collector
docker compose logs elasticsearch
```

Common causes:

- ELK profile not enabled
- collector still using `config.default.yaml`
- Elasticsearch not ready
- wrong data view
- application not sending OTLP logs

## No Traces In Grafana

Check:

```bash
docker compose logs tempo
docker compose logs otel-collector
```

Common causes:

- application not creating spans
- wrong OTLP endpoint
- Tempo datasource missing
- collector trace exporter failure

## High Memory Usage

- Elasticsearch needs more memory than core mode
- prefer default mode on small machines
- reduce retention or heap size when ELK is only needed temporarily

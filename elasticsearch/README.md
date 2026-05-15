# Elasticsearch

Phase 5 adds the ELK profile assets for logs:

- `index-templates/otel-logs-template.json`
- `ilm-policies/otel-logs-ilm-policy.json`

The current Compose profile starts Elasticsearch in local single-node mode with simplified security for development use.

When the `elk` profile starts, the `elasticsearch-init` service applies both assets automatically.

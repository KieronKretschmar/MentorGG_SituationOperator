# SituationOperator

Accesses a MatchDb to compute, store and serve Situations.

## Environment Variables
- `MYSQL_CONNECTION_STRING` : Connection string for the SituationDatabase. Defaults to InMemoryDatabase if unspecified
- `IS_MIGRATING` : Warning: Do not use in production. If this is set to true, migrations to SituationDatabase will be applied but nothing else. Only works if `MYSQL_CONNECTION_STRING` is also provided.
- `MATCH_DB_MYSQL_CONNECTION_STRING` : Connection string for the MatchDb from where data will be analyzed. Defaults to InMemoryDatabase if unspecified
- `AMQP_URI` : URI to the rabbit cluster [*]
- `AMQP_EXCHANGE_NAME` : Rabbits exchange name for finding MatchData-Fanout [*]
- `AMQP_EXCHANGE_CONSUME_QUEUE` : Rabbit queue's name for consuming messages from MatchData-Fanout 

[\*] *Required*


## Monitoring / Prometheus

Collect metrics on port `9913` (Defined in `Startup.cs:METRICS_PORT`) at `/metrics`

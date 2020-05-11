# SituationOperator

Accesses a MatchDb to compute, store and serve Situations.

## Environment Variables

**RabbitMQ**
- `MOCK_RABBIT` : Whether rabbit related services should be mocked and/or skipped to simplify startup. Only usable in Development.
- `AMQP_URI` : URI to the rabbit cluster [\*]
- `AMQP_EXCHANGE_NAME` : Rabbits exchange name for finding MatchData-Fanout [\*]
- `AMQP_EXCHANGE_CONSUME_QUEUE` : Rabbit queue's name for consuming messages from MatchData-Fanout 
- `AMQP_PREFETCH_COUNT` : Amount of messages to fetch before acknowledging the in-progress messages. Defaults to `1`.
- `AMQP_DEMOCENTRAL_REPLY` : Rabbit queue's name for producing messages to DemoCentral. [\*]

**Mysql - SituationDb**
- `MYSQL_CONNECTION_STRING` : Connection string for the SituationDatabase. Defaults to InMemoryDatabase if unspecified
- `IS_MIGRATING` : Warning: Do not use in production. If this is set to true, migrations to SituationDatabase will be applied but nothing else. Only works if `MYSQL_CONNECTION_STRING` is also provided.

**Redis**
- `MOCK_REDIS` : Whether a mocked redis connection should be used, making `REDIS_CONFIGURATION_STRING` unrequired. Only usable in Development.
- `REDIS_CONFIGURATION_STRING` : Comma-seperated options for the configuration of the MatchData Redis cache. [\*] 

**FileSystem**
- `EQUIPMENT_CSV_DIRECTORY` : Relative path to the directory containing the equipment .csv files. Defaults to `/app/data/equipment`.
- `ZONE_RESOURCES_DIRECTORY` : Relative path to the directory containing the zone resources files. Defaults to `/app/data/zones`.
[\*] *Required*


## Monitoring / Prometheus

Collect metrics on port `9913` (Defined in `Startup.cs:METRICS_PORT`) at `/metrics`

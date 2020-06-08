# SituationOperator

Computes, stores and serves Situations.

## Environment Variables

**RabbitMQ**
- `MOCK_RABBIT` : Whether rabbit related services should be mocked and/or skipped to simplify startup. Only usable in Development.
- `AMQP_URI` : URI to the rabbit cluster [\*]
- `AMQP_PREFETCH_COUNT` : Amount of messages to fetch before acknowledging the in-progress messages. Defaults to `1`.
- `AMQP_EXTRACTION_INSTRUCTIONS` : Rabbit queue's name for consuming instructions for extraction.
- `AMQP_EXTRACTION_REPLY` : Rabbit queue's name for producing messages to DemoCentral. [\*]

**MySQL - SituationDb**
- `MYSQL_CONNECTION_STRING` : Connection string for the SituationDatabase. Defaults to InMemoryDatabase if unspecified
- `IS_MIGRATING` : Warning: Do not use in production. If this is set to true, migrations to SituationDatabase will be applied but nothing else. Only works if `MYSQL_CONNECTION_STRING` is also provided.

**Redis**
- `MOCK_REDIS` : Whether a mocked redis connection should be used, making `REDIS_CONFIGURATION_STRING` unrequired. Only usable in Development.
- `REDIS_CONFIGURATION_STRING` : Comma-seperated options for the configuration of the MatchData Redis cache. [\*] 

**HTTP**
- `MATCHRETRIEVER_URL_OVERRIDE`: Override for MatchRetriever's URL.

**FileSystem**
- `EQUIPMENT_CSV_DIRECTORY` : Relative path to the directory containing the equipment .csv files. Defaults to `/app/data/equipment`.
- `ZONE_RESOURCES_DIRECTORY` : Relative path to the directory containing the zone resources files. Defaults to `/app/data/zones`.

[\*] *Required*

## Situations - Discussion

For each implemented or planned Situation, an issue will be created in GitLab. This issue serves as a place for discussion and collection of feedback regarding the issue.

## Analysis - Flow

Messages with instructions to analyze matches are consumed from a rabbit queue, and roughly the following steps are exectued:
- A `MatchDataSet` is fetched from redis.
- All implemented types of Situations are extracted by looking for patterns in the `MatchDataSet`.
- These situations as well as some metadata about the match are stored in the SituationDatabase.
- A Report is sent to DemoCentral.

## Accessing Data

Endpoints serve data from the SituationDatabase.

## Monitoring / Prometheus

Collect metrics on port `9913` (Defined in `Startup.cs:METRICS_PORT`) at `/metrics`

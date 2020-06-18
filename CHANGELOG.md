# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2020-06-17
### Added
- Influencer SubscriptionType for raising allowedRounds 1->2 

## [0.3.0] - 2020-06-12
### Added
- Many improvements regarding misplays logic

## [0.3.0] - 2020-06-08
### Added
- Add env var SKIP_REDIS
- Fallback for accessing MatchDataSet through MatchRetriever if Redis fails

### Changed
- Replace env var MOCK_REDIS with MOCK_MATCHDATASET_PROVIDER

## [0.2.0] - 2020-06-02
### Added
- Endpoint for situations of a specific type
- SkillDomains
- Replaced MatchSituationsModel.MatchId with MatchSituationsModel.MatchData
- SubscriptionConfigLoader

### Changed
- Remove exchange and rename queue name AMQP_EXCHANGE_CONSUME_QUEUE to AMQP_EXTRACTION_INSTRUCTIONS
- Remove exchange and rename queue name AMQP_DEMOCENTRAL_REPLY to AMQP_EXTRACTION_REPLY

## [0.1.0] - 2020-06-02
### Basic implementation

## [0.0.0] - 2020-04-23
### Create repo

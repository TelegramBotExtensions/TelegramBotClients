# Changelog

## [Unreleased]





## [1.0.4] - 2018-05-11

## Added

- warning disable for CA1068
- readonly modifiers on a lot of things thanks to @karb0f0s
- await to method calls in ReadMe usage example

## Changed

- Removed async/await from 'MakeRequestAsync' overloads
- The Obsolete behaviour on the default 'MakeRequestAsync' from error to warning

## Fixed

- 'UpdateQueue' in 'TelegramRequestScheduler' now removes itmes from the old queue when switching to general queue
- 'WaitOneInternalLocked' in 'TelegramRequestScheduler' now passes clientId to constructor of 'SchedulerRequestItem'
- Typo in SchedulerSettings causing SafePrivateChatInterval to be set to SafeGeneralInterval


## [1.0.3] - 2018-05-11

### Fixed

- Catching 'getUpdates' in the 'MakingApiRequest' event since the base class will not call our version of 'MakeRequestAsync'


## [1.0.2] - 2018-05-11

### Added

- Required usings to ReadMe
- 'SchedulingMethod' use example to ReadMe

### Changed

- GetUpdates requests now all use 'SchedulingMethod.NoScheduling' to avoid not receiving updates


## [1.0.1] - 2018-05-11

### Added

- Changelog

### Fixed

- Implemented the fix for 'EditMessageTextAsync' - https://github.com/TelegramBots/Telegram.Bot/pull/700


## [1.0.0] - 2018-05-11

### Aded

- BlockingTelegramBotClient
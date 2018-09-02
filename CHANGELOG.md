# Changelog

## [1.0.6] - 2018-09-02

### Added

- Unit tests for the 'BlockingClient'
- The unit test project to the 'TelegramBotClients' solution
- A unit test project (@karb0f0s)

### Changed

- Updated BlockingTelegramBotClient to Telegram.Bot 14.9.0
- Refactored the 'ChatRequestCount' code (@karb0f0s)
- Refactored the 'TelegramRequestScheduler' code (@karb0f0s)
- Moved repository to TelegramBotExtensions organization
- Moved suppression of CA1068 to the GlobalSuppressions.cs file


## [1.0.5] - 2018-05-12

### Changed

- Reduced the 'SafeGeneralInterval' by 1 ms
- Reduced MaxBurst values of 'SchedulerSettings'


## [1.0.4] - 2018-05-11

### Added

- warning disable for CA1068
- readonly modifiers on a lot of things (@karb0f0s)
- await to method calls in ReadMe usage example (@karb0f0s)

### Changed

- Removed async/await from 'MakeRequestAsync' overloads (@karb0f0s)
- The Obsolete behaviour on the default 'MakeRequestAsync' from error to warning

### Fixed

- 'UpdateQueue' in 'TelegramRequestScheduler' now removes itmes from the old queue when switching to general queue
- 'WaitOneInternalLocked' in 'TelegramRequestScheduler' now passes clientId to constructor of 'SchedulerRequestItem' (@karb0f0s)
- Typo in SchedulerSettings causing SafePrivateChatInterval to be set to SafeGeneralInterval (@karb0f0s)


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

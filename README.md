# 🎄 Shakey's AoC 2022 🌟

[![CI workflow](https://github.com/robshakespeare/aoc2022/actions/workflows/CI-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2022/actions/workflows/CI-workflow.yml)
[![Azure Static Web App Deployment](https://github.com/robshakespeare/aoc2022/actions/workflows/deployment-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2022/actions/workflows/deployment-workflow.yml)

Rob Shakespeare's solutions to the Advent of Code 2022 challenges at https://adventofcode.com/2022.

[Go to Shakey's AoC 2022 web app.](https://black-smoke-0bf67c303.azurestaticapps.net)


### Prerequisites

* [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Optional: to be able to run the cake scripts, first: `dotnet tool restore`


### Run

To run the console application:

```
dotnet run --project AoC.CLI
```

To run the Blazor WebAssembly application:

```
dotnet run --project AoC.WasmUI
```

To run the MAUI app in an Android Emulator, start an [Android Emulator](https://visualstudio.microsoft.com/vs/msft-android-emulator/), then run:

```
dotnet build AoC.MAUI -t:Run -f net7.0-android
```


### Test

```
dotnet test
```

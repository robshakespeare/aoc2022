# 🎄 Shakey's AoC 2022 🌟

[![CI workflow](https://github.com/robshakespeare/aoc2022/actions/workflows/CI-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2022/actions/workflows/CI-workflow.yml)
[![Azure Static Web App Deployment](https://github.com/robshakespeare/aoc2022/actions/workflows/deployment-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2022/actions/workflows/deployment-workflow.yml)
[![App releases](https://github.com/robshakespeare/aoc2022/actions/workflows/release-workflow.yml/badge.svg)](https://github.com/robshakespeare/aoc2022/actions/workflows/release-workflow.yml)

Rob Shakespeare's solutions to the Advent of Code 2022 challenges at https://adventofcode.com/2022.


### Shakey's AoC 2022 Releases
* [Blazor WebAssembly App](https://black-smoke-0bf67c303.azurestaticapps.net)
* [Andriod App download](https://github.com/robshakespeare/aoc2022/releases/latest/download/com.rws.aoc2022-Signed.apk)
* [Windows CLI download](https://github.com/robshakespeare/aoc2022/releases/latest/download/AoC.CLI.exe)


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

To decrypt the puzzle inputs in this repository (requires `AocPuzzleInputCryptoKey` config value):

```
dotnet run --project AoC.CLI --decrypt
```


### Test

```
dotnet test
```

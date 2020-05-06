﻿This sample demonstrates using [Adaptive dialog][1],  [Language Generation][2] PREVIEW features with [LUIS][5] to demonstrate an end-to-end ToDo bot in action.

This sample uses preview packages available on the [BotBuilder MyGet feed][4].

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```
- Configure the necessary [LUIS applications](#LUIS-Setup) required to run this sample

## To try this sample

- Clone the repository

    ```bash
    git clone https://github.com/Microsoft/botbuilder-samples.git
    ```
- In a terminal, navigate to `experimental/adaptive-dialog/csharp_dotnetcore/todo-bot`
- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `experimental/adaptive-dialog/csharp_dotnetcore/todo-bot` folder
  - Select `ToDoBotWithLUIS.csproj` file
  - Press `F5` to run the project

  
## To debug adaptive dialogs
- You can install and use [this visual studio code extension][extension] to debug Adaptive dialogs. 

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.3.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## LUIS Setup
### Using CLI
- Install [nodejs][2] version 10.14 or higher
- Install required CLI tools
```bash
> npm i -g luis-apis @microsoft/botframework-cli
```
- In a command prompt, navigate to `botbuilder-samples/experimental/adaptive-dialog/csharp_dotnetcore/todo-bot`
- To cross-train all LUIS applications for this bot
```bash
> bf luis:cross-train --in dialogs --out generated --config Dialogs/DialogLuHierarchy.config.json
```
- To create, train and pubish LUIS applications for this bot
```bash
> bf luis:build --in generated --out generated --authoringKey <Your LUIS Authoring key>
```

[1]:../../README.md
[2]:../../language-generation/README.md
[3]:../../../../samples/csharp_dotnetcore/06.using-cards
[4]:https://botbuilder.myget.org/gallery/botbuilder-declarative
[5]:https://luis.ai
[6]:#LUIS-Setup
[7]:https://github.com/Microsoft/botbuilder-tools
[8]:https://nodejs.org/en/
[9]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-how-to-account-settings#authoring-key
[10]:https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys
[extension]:https://marketplace.visualstudio.com/items?itemName=tomlm.vscode-dialog-debugger
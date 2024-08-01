# ReplConsole

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![Build Status](https://github.com/afluegge/ReplConsole/actions/workflows/build.yml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afluegge_ReplConsole&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=afluegge_ReplConsole)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=afluegge_ReplConsole&metric=coverage)](https://sonarcloud.io/summary/new_code?id=afluegge_ReplConsole)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afluegge_ReplConsole&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=afluegge_ReplConsole)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=afluegge_ReplConsole&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=afluegge_ReplConsole)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=afluegge_ReplConsole&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=afluegge_ReplConsole)

## Overview

Welcome to **ReplConsole**! This project is a .NET 8.0-based command-line tool that provides a simple yet powerful REPL (Read-Eval-Print Loop) console. You can easily extend the functionality by adding your custom commands, making it perfect for automating tasks, experimenting with code snippets, or creating interactive tools.

## Getting Started

To get started with ReplConsole, clone the repository and build the project using .NET 8.0. You can then run the console and start entering commands. The default prompt is set to `>>`, and you can customize the console's title, prompt, and more via the `appsettings.json` file.

## Configuration

The REPL console settings are defined in the `ReplConsole` section of the `appsettings.json` file. Here’s a brief overview of the configuration options:

- **Environment**: Specifies the environment (e.g., Development, Production).
- **AppVersion**: Displays the version of the executing assembly.
- **AppName**: Sets the name of the application (default is "ReplConsole").
- **ConsoleTitle**: Sets the title of the console window.
- **Prompt**: Defines the command prompt string.
- **CommandAssemblies**: A list of assembly names containing custom command handlers.

### Sample Configuration

```json
{
  "ReplConsole": {
    "Environment": "Development",
    "AppVersion": "1.0.0",
    "AppName": "ReplConsole",
    "ConsoleTitle": "My Custom REPL Console",
    "Prompt": ">>",
    "CommandAssemblies": [ "CustomCommandsAssembly" ]
  }
}
```

## Extending with Custom Commands

Custom commands in ReplConsole are easy to add. You can implement them directly within the project or in a separate assembly. If you choose the latter, make sure the custom command assembly is in the same directory as the executing assembly and register its name in the `CommandAssemblies` section of `appsettings.json`.

### Creating a Custom Command

To create a custom command, you need to extend the `CommandHandlerBase` class and implement the required properties and methods. Here’s an example:

```csharp
public class HelloWorldCommandHandler(ILogger<HelloWorldCommandHandler> logger, IReplConsole console) : CommandHandlerBase(logger, console)
{
    public override string Name => "hello";
    public override string Description => "Prints a nice greeting.";

    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        ReplConsole.WriteLine($"Hello: {string.Join(", ", args)}");
        return ValueTask.CompletedTask;
    }
}
```

In this example, the `HelloWorldCommandHandler` defines a command named `hello` that prints a greeting message to the console. The `HandleCommand` method is where you implement the logic for your command.

### Registering Custom Commands

Once you've implemented your custom command, add the name of its assembly to the `CommandAssemblies` array in `appsettings.json`. For instance, if your custom commands are in an assembly named `CustomCommandsAssembly.dll`, your configuration would look like this:

```
{
  "ReplConsole": {
    ...
    "CommandAssemblies": [ "CustomCommandsAssembly" ]
  }
}
```

## Conclusion

ReplConsole is a flexible and extensible REPL tool for .NET applications. Whether you want to quickly test code snippets, automate repetitive tasks, or develop a full-featured interactive console application, ReplConsole has you covered. Dive in, create some custom commands, and see how you can enhance your workflows!

Feel free to contribute to the project or submit issues. Happy coding!
<br/>
<br/>
<br/>

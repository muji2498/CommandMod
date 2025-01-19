# Console Command - Nuclear Option

## Bepinex Version
This mod requires bepinex version [5.4.23.2](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2)

## Notes
The mod will reply back showing the calling players name, i intend to replace this with something like `Server` or `Host`, this could be a configurable name aswell. 

## Config Overview

The config for this mod can be found at `GameInstallDir\BepInEx\config\me.muj.commandmod.cfg`

`Prefix` - This Config will set the prefix that is used to trigger command. Default is `!`

# Built in commands

`ping` - Will message `pong` back to the player that called the command. <br>
`listplayers` - Will message a comma seperated list of the players connected to the server. <br>
`list` - Will list the commands in the Bepinex **Console** [Click here to see how to enable console](https://docs.bepinex.dev/articles/user_guide/troubleshooting.html) <br>
`kick playername|steamid` - Will kick the player specified. <br>
`addmoney @me|steamid|name <amount>` - Will add money to the player specified. <br>
`ban steamid` - Will ban the steamid specified. <br>
`unban steamid` - Will unban the steamid specified. <br>
`setfps` - Will set the fps of the server.

# Plugin Usage

To register a command you must reference `CommandMod.dll` in your project and set CommandMod as a bepinex dependency. Once that is done you can use the `ConsoleCommand` attribute. 
The command plugin will then automatically find and register these command. <br>

```csharp
[BepInDependency("CommandMod", BepInDependency.DependencyFlags.HardDependency)]
```

> **TIP:** if you are making multiple commands that can be grouped together then prefix them with a common name, sort of like this `test.command1` `test.command2` <br/> 

`CommandName` : Required - The name of the command. <br> 
`OnlyHost` : Default = false - Flag that only allows the host to run the command <br>
`Roles` : Default = Roles.None - The Role(s) needed inorder to execute the command.

>**NOTE:** Commands must have the parameters shown below, they can be named anything. In the args array it will pass in the arguments of the command, 
> for example if a command is `kick <playername>` you can read the player name by simply reading args[0].
> <br/>
> You can access the object of the player who called the command by doing `arg2.Player` <br>
> <br>
> The command you want to register <b style="color:red;">MUST</b> be static.

```csharp
[ConsoleCommand("test"]
private static void TestCommand(string[] args, CommandObjects arg2)
{
    // logic here
}

[ConsoleCommand("test", onlyHost=true]
private static void TestCommand(string[] args, CommandObjects arg2)
{
    // logic here
}

[ConsoleCommand("test", roles: Roles.Moderator | Roles.Owner | Roles.Admin)]
private static void TestCommand(string[] args, CommandObjects arg2)
{
    // logic here
}
```

# Roles

Multiple roles can be assigned using a bitwise OR (|). For example, a command can have the requirement `Roles.Owner | Roles.Admin` 
meaning that only players with Owner roles and Admin roles can execute the command, this will show up in the config as `3`


```csharp
public enum Roles
{
    None = 0,
    Owner = 1,
    Admin = 2,
    Moderator = 4,
}
```

# Errors

`Skipping. Invalid command attribute: {attribute} - {method.Name} in {assembly.GetName().Name} Please Update the command attribute first.` <br>
If you are seeing this error in your console, it is due to the fact that your `ConsoleCommand` is mismatched with the one that is shipped with the ConsoleMod plugin, 
in order to fix this you will need to rebuild your plugin with the latest ConsoleMod plugin referenced.

`Failed to create delegate for command {commandName}, method needs to be static: {ex.Message}` <br>
This error will show if the command you try to register is not static.

## Building Project

Change `<GameDir>YourGameDirectoryHere</GameDir>` inside GameDir.targets to the install location of your game. <br>
Example: `<GameDir>C:\Program Files (x86)\Steam\steamapps\common\Nuclear Option</GameDir>`
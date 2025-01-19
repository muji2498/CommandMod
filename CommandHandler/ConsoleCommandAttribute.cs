using System;

namespace CommandMod.CommandHandler;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string CommandName { get; }
    public Roles Roles { get; }

    public ConsoleCommandAttribute(string commandName, Roles roles = Roles.None)
    {
        CommandName = commandName;
        Roles = roles;
    }
}
using System;

namespace CommandMod.CommandHandler;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string CommandName { get; }
    public bool OnlyHost { get; }
    public Roles Roles { get; }

    public ConsoleCommandAttribute(string commandName, bool onlyHost = false, Roles roles = Roles.None)
    {
        CommandName = commandName;
        OnlyHost = onlyHost;
        Roles = roles;
    }
}
using System;

namespace CommandMod.CommandHandler;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ConsoleCommandAttribute : Attribute
{
    public string CommandName { get; }
    public bool OnlyHost { get; }
    public string RequiredPermission { get; }

    public ConsoleCommandAttribute(string commandName, bool onlyHost  = false, string requiredPermission = "*")
    {
        CommandName = commandName;
        OnlyHost = onlyHost;
        RequiredPermission = requiredPermission;
    }
}
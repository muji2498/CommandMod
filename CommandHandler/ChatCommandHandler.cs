using System;
using System.Collections.Generic;
using System.Linq;
using Mirage;

namespace CommandMod.CommandHandler;

public class ChatCommandHandler
{
    private static Dictionary<string, CommandMetaData> _commands = new();
    private static Dictionary<string, Type> _commandNamesAndTypes = new();
    public static Dictionary<string, CommandMetaData> Commands => _commands;
    
    private PermissionConfig _permissionConfig;

    public ChatCommandHandler(PermissionConfig permissionConfig)
    {
        _permissionConfig = permissionConfig;
    }
    
    public void RegisterCommand(Type type, string name, Action<string[], CommandObjects> action, Roles roles)
    {
        // command was already registered so return early
        if (ReplaceCommand(type, name, action, roles))
        { 
            Plugin.Logger.LogInfo($"Refreshed Command: {name} - Permission: {roles}");
            return;
        }
        
        if (_commands.ContainsKey(name))
        {
            name = $"{type.Assembly.GetName().Name.ToLower()}.{name}";
        }
        
        _commands.Add(name, new CommandMetaData
        {
            Action = action, 
            Roles = roles
        });
        
        _commandNamesAndTypes.Add(name, type);
        
        Plugin.Logger.LogInfo($"Registered Command: {name} - Permission: {roles}");
    }

    /// <summary>
    /// method will check if command was registered before if it was then it will replace the action
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="roles"></param>
    /// <returns>True if a commands action was replaced</returns>
    public bool ReplaceCommand(Type type, string name, Action<string[], CommandObjects> action, Roles roles)
    {
        // have i registered this command before?
        if (_commandNamesAndTypes.ContainsKey(name) || _commands.ContainsKey($"{type.Assembly.GetName().Name.ToLower()}.{name}"))
        {
            // i have, so replace my action
            var commandMetaData = _commands[name];
            commandMetaData.Action = action;
            return true;
        }
        return false;
    }

    public void ExecuteCommand(string input, CommandObjects command)
    {
        var player = command.Player;
        INetworkPlayer owner = player.Owner;
        
        string[] commandParts = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (commandParts.Length == 0)
        {
            var message = "Message Not Valid";
            Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
        }
        
        string commandName = commandParts[0].Substring(1).ToLower();
        string[] args = commandParts.Skip(1).ToArray();

        if (!_commands.TryGetValue(commandName, out CommandMetaData metaData))
        {
            var suggestions = _commands.Keys.FindNearestStrings(commandName);
            var suggestionMessage = suggestions.Any()
                ? $"Couldn't find command named {commandName}. Did you mean: {string.Join(", ", suggestions)}?" 
                : $"Couldn't find command named {commandName}.";
            Wrapper.ChatManager.TargetReceiveMessage(owner, suggestionMessage, player, false);
        }
        else
        {
            // role is none so anyone can execute
            if (metaData.Roles == Roles.None)
            {
                Plugin.Logger.LogInfo($"Executing command: {commandName} {string.Join(" ", args)}");
                metaData.Action(args, command);
                return;
            }

            if (!_permissionConfig.HasRole(player.SteamID, metaData.Roles))
            {
                var message = "You do not have permission to use this command.";
                Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
                return;
            }

            Plugin.Logger.LogInfo($"({player.PlayerName}) Executing command: {commandName} {string.Join(" ", args)}");
            metaData.Action(args, command);
        }
    }
    
    public struct CommandMetaData
    {
        public Action<string[], CommandObjects> Action { get; set; }
        public Roles Roles { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Mirage;

namespace CommandMod.CommandHandler;

public class ChatCommandHandler
{
    private static Dictionary<string, CommandMetaData> _commands = new();
    public static Dictionary<string, CommandMetaData> Commands => _commands;
    
    private PermissionConfig _permissionConfig;

    public ChatCommandHandler(PermissionConfig permissionConfig)
    {
        _permissionConfig = permissionConfig;
    }
    
    public void RegisterCommand(string name, Action<string[], CommandObjects> action, Roles roles)
    {
        if (_commands.ContainsKey(name))
        {
            Plugin.Logger.LogError($"Command {name} is already registered");
            return;
        }
        
        _commands.Add(name.ToLower(), new CommandMetaData
        {
            Action = action, 
            Roles = roles
        });
        Plugin.Logger.LogInfo($"Registered Command: {name} - Permission: {roles}");
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
            var message = $"Couldn't find command named {commandName}";
            Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
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
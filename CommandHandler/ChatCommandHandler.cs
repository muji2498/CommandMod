using System;
using System.Collections.Generic;
using System.Linq;
using Mirage;

namespace CommandMod.CommandHandler;

public class ChatCommandHandler
{
    private static Dictionary<string, CommandMetaData> _commands = new();
    private PermissionConfig _permissionConfig;

    public ChatCommandHandler(PermissionConfig permissionConfig)
    {
        _permissionConfig = permissionConfig;
    }
    
    public void RegisterCommand(string name, Action<string[], CommandObjects> action, bool onlyHost, string requiredPermission)
    {
        _commands.Add(name.ToLower(), new CommandMetaData
        {
            Action = action, 
            OnlyHost = onlyHost,
            RequiredPermission = requiredPermission
        });
        Plugin.Logger.LogInfo($"Registered Command: {name} - IsHostOnly: {onlyHost} - Permission: {requiredPermission}");
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
        
        if (_commands.TryGetValue(commandName, out CommandMetaData metaData))
        {
            if (metaData.OnlyHost && !player.IsHost)
            {
                Plugin.Logger.LogInfo($"Ignoring Command: {commandName} - Only Host: {player.IsHost}");
                
                var message = "This command can only be executed by the host.";
                Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
                return;
            }

            if (!_permissionConfig.HasPermission(player.SteamID, metaData.RequiredPermission))
            {
                var message = "You do not have permission to use this command.";
                Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
                return;
            }
            
            Plugin.Logger.LogInfo($"({player.PlayerName}) Executing command: {commandName} {string.Join(" ", args)}");
            metaData.Action(args, command);
        }
        else
        {
            var message = "Couldn't find any command";
            Wrapper.ChatManager.TargetReceiveMessage(owner, message, player, false);
        }
    }

    public static Dictionary<string, CommandMetaData> GetCommands()
    {
        return _commands;
    }
    
    public struct CommandMetaData
    {
        public Action<string[], CommandObjects> Action { get; set; }
        public bool OnlyHost { get; set; }
        public string RequiredPermission { get; set; }
    }
}
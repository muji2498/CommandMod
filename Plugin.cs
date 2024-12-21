using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;

namespace CommandMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    public new static ManualLogSource Logger { get; private set; } = BepInEx.Logging.Logger.CreateLogSource("CommandMod");
    public ChatCommandHandler CommandHandler;
    private PermissionConfigManager _permissionConfigManager;
    public new static Config Config { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        
        Config = new Config(base.Config);

        _permissionConfigManager = new PermissionConfigManager(this);
        CommandHandler = new ChatCommandHandler(_permissionConfigManager.Config);
        
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }
    
    [ConsoleCommand("list")]
    private static void ListCommand(string[] args, CommandObjects context)
    {
        var callingPlayer = context.Player;
        
        // Create the complete message
        var message = new StringBuilder();
        foreach (var command in ChatCommandHandler.GetCommands())
        {
            message.Append($"{command.Key} - isHostOnly? {command.Value.OnlyHost}\n");
        }
    
        // Break the message into chunks of 128 characters
        var fullMessage = message.ToString();
        var chunks = new List<string>();
        for (int i = 0; i < fullMessage.Length; i += 128)
        {
            int chunkLength = Math.Min(128, fullMessage.Length - i);
            chunks.Add(fullMessage.Substring(i, chunkLength));
        }
    
        // Send each chunk as a separate message
        foreach (var chunk in chunks)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, chunk, callingPlayer, false);
        }
    }
    
    [ConsoleCommand("steamid", true)]
    private static void SteamIdCommand(string[] args, CommandObjects arg2)
    {
        var callingPlayer = arg2.Player;
        
        var message = $"Invalid parameter amount needs to be like: (steamid name)";

        if (args.Length == 0)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, message, callingPlayer, false);
            return;
        }
        
        var targetPlayer = args[0];
        try
        {
            var player = FindPlayerByName(targetPlayer);
            message = $"{player.PlayerName} steamId is {player.SteamID}";
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, message, player, false);
        }
        catch (Exception e)
        {
            message = $"Couldn't find player: Command given (steamid {targetPlayer})";
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, message, callingPlayer, false);
        }
    }

    [ConsoleCommand("addmoney", true)]
    private static void AddMoneyCommand(string[] args, CommandObjects arg2)
    {
        var callingPlayer = arg2.Player;
        var message = $"Invalid parameter amount needs to be like: (addmoney @me|steamid|name <amount>)";

        if (args.Length == 0)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, message, callingPlayer, false);
            return;
        }
        
        var targetPlayer = args[0];
        var amount = float.Parse(args[1]);
        try
        {
            if (targetPlayer == "@me")
            {
                callingPlayer.AddAllocation(amount);
            }
            else if (targetPlayer.StartsWith("76"))
            {
                var steamId = ulong.Parse(targetPlayer);
                var player = FindPlayerBySteamId(steamId);
                player.AddAllocation(amount);
            }
            else
            {
                var player = FindPlayerByName(targetPlayer);
                player.AddAllocation(amount);
            }
        }
        catch (Exception e)
        {
            message = $"Couldn't find player: Command given (addmoney {targetPlayer} {amount})";
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, message, callingPlayer, false);
        }
    }

    private static Player FindPlayerBySteamId(ulong steamId)
    {
        var first = UnitRegistry.playerLookup.First(p => p.Value.SteamID == steamId);
        return first.Value;
    }

    private static Player FindPlayerByName(string targetPlayer)
    {
        var first = UnitRegistry.playerLookup.First(p => p.Value.PlayerName.Contains(targetPlayer));
        return first.Value;
    }

    [ConsoleCommand("test")]
    private static void TestCommand(string[] args, CommandObjects arg2)
    {
        var player = arg2.Player;
        var message = "Thanks for the message";
        Wrapper.ChatManager.TargetReceiveMessage(player.Owner, message, player, false);
        
    }
}
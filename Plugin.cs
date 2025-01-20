using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;
using NuclearOption.Networking;
using UnityEngine;

namespace CommandMod;

[BepInPlugin("me.muj.commandmod", "CommandMod","1.0.3")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    public new static ManualLogSource Logger { get; private set; } = BepInEx.Logging.Logger.CreateLogSource("CommandMod");
    public ChatCommandHandler CommandHandler;
    public PermissionConfigManager PermissionConfigManager;
    public new static Config Config { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        
        Config = new Config(base.Config);

        PermissionConfigManager = new PermissionConfigManager();
        CommandHandler = new ChatCommandHandler(PermissionConfigManager.Config);
        
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

    [ConsoleCommand("ping")]
    public static void Ping(string[] args, CommandObjects arg2)
    {
        Wrapper.ChatManager.TargetReceiveMessage(arg2.Player.Owner, "Pong", arg2.Player, false);
    }
    
    [ConsoleCommand("listplayers")]
    public static void ListPlayers(string[] args, CommandObjects arg2)
    {
        var callingPlayer = arg2.Player;
        StringBuilder sb = new StringBuilder();
        foreach (var player in UnitRegistry.playerLookup)
        {
            sb.Append($"{player.Value.PlayerName}, ");
        }
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, sb.ToString(), callingPlayer, false);
    }

    [ConsoleCommand("list", Roles.Owner)]
    public static void ListCommands(string[] args, CommandObjects arg2)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var command in ChatCommandHandler.Commands.Keys)
        {
            sb.Append($"{command}, ");
        }
        Logger.LogInfo(sb.ToString());
    }

    [ConsoleCommand("kick", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void KickPlayer(string[] args, CommandObjects arg2)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(arg2.Player.Owner, "Usage: [kick playername|steamid]", arg2.Player, false);
            return;
        }
        var callingPlayer = arg2.Player;
        var targetPlayer = args[0];
        var playerToKick = Utils.IdentifyPlayer(targetPlayer);
        if (playerToKick == null)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Player not found.", callingPlayer, false);
            return;
        }
        NetworkManagerNuclearOption.i.KickPlayerAsync(playerToKick);
    }
    
    [ConsoleCommand("ban", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Ban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [ban steamid]", context.Player, false);
            return;
        }

        var steamId = args[0];
        if (!ulong.TryParse(steamId, out var id))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Invalid ulong. Ensure it's a numeric value.", context.Player, false);
            return;
        }

        var playerIdField = AccessTools.Field(typeof(BlockList), "playerId");
        if (playerIdField == null)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Could not access block list.", context.Player, false);
            return;
        }
        
        var bannedPlayers = playerIdField.GetValue(null) as List<ulong> ?? new List<ulong>();
        if (bannedPlayers.Contains(id))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Player is already banned.", context.Player, false);
            return;
        }
        
        bannedPlayers.Add(id);
        playerIdField.SetValue(null, bannedPlayers);
        
        Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Player {id} banned.", context.Player, false);
    }
    
    [ConsoleCommand("unban",  Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Unban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [unban steamid]", context.Player, false);
            return;
        }

        var steamId = args[0];
        if (!ulong.TryParse(steamId, out var id))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Invalid ulong. Ensure it's a numeric value.", context.Player, false);
            return;
        }

        var playerIdField = AccessTools.Field(typeof(BlockList), "playerId");
        if (playerIdField == null)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Could not access block list.", context.Player, false);
            return;
        }
        
        var bannedPlayers = playerIdField.GetValue(null) as List<ulong> ?? new List<ulong>();
        if (!bannedPlayers.Contains(id))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Player is not banned.", context.Player, false);
            return;
        }
        
        bannedPlayers.Remove(id);
        playerIdField.SetValue(null, bannedPlayers);
        
        Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, $"Player {id} unbanned.", context.Player, false);
    }

    [ConsoleCommand("setfps", Roles.Owner)]
    public static void SetFPS(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Usage: [setfps amount]", context.Player, false);
            return;
        }

        if (!int.TryParse(args[0], out int amount))
        {
            Wrapper.ChatManager.TargetReceiveMessage(context.Player.Owner, "Invalid amount. please use a numeric value", context.Player, false);
            return;
        }
        
        QualitySettings.vSyncCount = 0; 
        PlayerPrefs.SetInt("Vsync", 0);
        Application.targetFrameRate = amount;
    }
}
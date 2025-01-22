using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;
using NuclearOption.Networking;
using UnityEngine;

namespace CommandMod;

[BepInPlugin("me.muj.commandmod", "CommandMod","1.0.4")]
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
        Logger.LogInfo($"Plugin me.muj.commandmod is loaded!");
        
        var harmony = new Harmony("me.muj.commandmod");
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
}
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using CommandMod.Extensions;
using HarmonyLib;

namespace CommandMod;

[BepInPlugin("me.muj.commandmod", "CommandMod","2.0.1")]
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
        arg2.Player.SendChatMessage("Pong");
    }
    
    [ConsoleCommand("listplayers")]
    public static void ListPlayers(string[] args, CommandObjects arg2)
    {
        var callingPlayer = arg2.Player;
        var playerList = string.Join(", ", UnitRegistry.playerLookup.Select(player => player.Value.PlayerName));
        callingPlayer.SendChatMessage(playerList);
    }

    [ConsoleCommand("list", Roles.Owner)]
    public static void ListCommands(string[] args, CommandObjects arg2)
    {
        var commandsList = string.Join(", ", ChatCommandHandler.Commands.Keys);
        Logger.LogInfo(commandsList);
    }
}
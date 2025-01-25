using System.Linq;
using BepInEx;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;

namespace CommandMod;

[BepInPlugin("me.muj.commandmod", "CommandMod","1.0.5")]
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
        var playerList = string.Join(", ", UnitRegistry.playerLookup.Select(player => player.Value.PlayerName));
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, playerList, callingPlayer, false);
    }

    [ConsoleCommand("list", Roles.Owner)]
    public static void ListCommands(string[] args, CommandObjects arg2)
    {
        var commandsList = string.Join(", ", ChatCommandHandler.Commands.Keys);
        Logger.LogInfo(commandsList);
    }
}
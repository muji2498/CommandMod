using CommandMod.CommandHandler;
using HarmonyLib;

namespace CommandMod.Patches;

public class MainMenuPatches
{
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class Start
    {
        static bool Prefix()
        {
            CommandLoader.RegisterCommandsFromPlugins(Plugin.Instance.CommandHandler);
            return true;
        }
    }
}
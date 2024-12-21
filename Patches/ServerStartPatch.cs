using CommandMod.CommandHandler;
using HarmonyLib;
using NuclearOption.Networking;

namespace CommandMod.Patches;

public class ServerStartPatches
{
    [HarmonyPatch(typeof(NetworkManagerNuclearOption), nameof(NetworkManagerNuclearOption.StartHost))]
    public class StartHost
    {
        static bool Prefix()
        {
            CommandLoader.RegisterCommandsFromPlugins(Plugin.Instance.CommandHandler);
            return true;
        }
    }
}
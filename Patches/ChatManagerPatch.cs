using CommandMod.CommandHandler;
using HarmonyLib;
using Mirage;
using NuclearOption.Networking;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), "CmdSendChatMessage")]
    public class CmdSendChatMessage
    {
        static bool Prefix(string message, bool allChat, INetworkPlayer sender = null)
        {
            if (!message.StartsWith(Plugin.Config.Prefix.Value)) return true;
            
            var objects = new CommandObjects { Player = NetworkManagerNuclearOption.i.Server.LocalPlayer.Identity.GetComponent<Player>() };
            Plugin.Instance.CommandHandler.ExecuteCommand(message, objects);
            return false;
        }
    }
}
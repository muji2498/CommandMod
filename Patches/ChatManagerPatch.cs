using CommandMod.CommandHandler;
using HarmonyLib;
using Mirage;
using NuclearOption.Networking;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), "UserCode_CmdSendChatMessage_1323305531")]
    public class TargetReceiveMessage
    {
        static bool Prefix(ChatManager __instance, string message, bool allChat, INetworkPlayer sender)
        {
            if (!message.StartsWith(Plugin.Config.Prefix.Value)) return true;
            
            Player player;
            if (!sender.TryGetPlayer(out player))
                return true;
            
            var objects = new CommandObjects { Player = player };
            Plugin.Instance.CommandHandler.ExecuteCommand(message, objects);
            return false;
        }
    }
}
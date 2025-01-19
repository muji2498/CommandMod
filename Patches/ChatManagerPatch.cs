using CommandMod.CommandHandler;
using HarmonyLib;
using Mirage;
using Mirage.RemoteCalls;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), "TargetReceiveMessage")]
    public class TargetReceiveMessage
    {
        static bool Prefix(ChatManager __instance, INetworkPlayer _, string message, Player player, bool allChat)
        {
            if (ClientRpcSender.ShouldInvokeLocally(__instance, RpcTarget.Player, _, false))
            {
                if (!message.StartsWith(Plugin.Config.Prefix.Value)) return true;

                var objects = new CommandObjects { Player = player };
                Plugin.Instance.CommandHandler.ExecuteCommand(message, objects);
                return false;
            }

            return true;
        }
    }
}
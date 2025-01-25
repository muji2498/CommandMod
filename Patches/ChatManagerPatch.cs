using System;
using BepInEx.Logging;
using CommandMod.CommandHandler;
using HarmonyLib;
using Mirage;
using Mirage.RemoteCalls;

namespace CommandMod.Patches;

public class ChatManagerPatch
{
    [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.TargetReceiveMessage))]
    public class TargetReceiveMessage
    {
        private static bool SafeShouldInvokeLocally(NetworkBehaviour behaviour, RpcTarget target, INetworkPlayer player, bool excludeOwner) {
            try {
                return ClientRpcSender.ShouldInvokeLocally(behaviour, target, player, excludeOwner);
            } catch (Exception) {
                return false;
            }
        }

        static bool Prefix(ChatManager __instance, INetworkPlayer _, string message, Player player, bool allChat)
        {
            if (!message.StartsWith(Plugin.Config.Prefix.Value)) return true;

            if (SafeShouldInvokeLocally(__instance, RpcTarget.Player, _, false))
            {
                var objects = new CommandObjects { Player = player };
                Plugin.Instance.CommandHandler.ExecuteCommand(message, objects);
            }

            return false;
        }
    }
}
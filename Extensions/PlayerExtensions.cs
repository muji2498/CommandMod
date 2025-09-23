using NuclearOption.Networking;

namespace CommandMod.Extensions;

public static class PlayerExtensions
{
    public static void SendChatMessage(this Player player, string message)
    {
        Wrapper.ChatManager.TargetReceiveMessage(player.Owner, message, player, false);
    }
}
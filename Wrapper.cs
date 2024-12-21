using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace CommandMod;

public class Wrapper
{
    private static FieldInfo _chatManager = AccessTools.Field(typeof(ChatManager), "i");
    public static ChatManager ChatManager
    {
        get => (ChatManager)_chatManager.GetValue(null);
        set
        {
            if (value != null)
                _chatManager.SetValue(null, value);
        }
    }
    
    private static FieldInfo _blockList = AccessTools.Field(typeof(BlockList), "playerId");

    public static List<ulong> BlockList
    {
        get => _blockList.GetValue(null) as List<ulong>;
        set
        {
            if (value != null)
                _blockList.SetValue(null, value);
        }
    }
}
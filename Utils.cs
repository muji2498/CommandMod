using System;
using System.Linq;

namespace CommandMod;

public class Utils
{
    public static Player IdentifyPlayer(string identifier)
    {
        if (identifier.StartsWith("76"))
        {
            if (ulong.TryParse(identifier, out var id))
            {
                return FindPlayerBySteamId(id);
            }
            throw new FormatException("Invalid Steam ID Format");
        }
        
        return FindPlayerByName(identifier);
    }
    
    public static Player FindPlayerBySteamId(ulong steamId)
    {
        var first = UnitRegistry.playerLookup.First(p => p.Value.SteamID == steamId);
        return first.Value;
    }

    public static Player FindPlayerByName(string targetPlayer)
    {
        var first = UnitRegistry.playerLookup.First(p => p.Value.PlayerName.Contains(targetPlayer));
        return first.Value;
    }
}
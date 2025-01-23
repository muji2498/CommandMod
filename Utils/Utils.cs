using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandMod;

public class Utils
{
    public static List<Player> IdentifyPlayer(string identifier)
    {
        if (identifier.StartsWith("76"))
        {
            if (ulong.TryParse(identifier, out var id))
            {
                var player = FindPlayerBySteamId(id);
                return player != null ? [player] : null;
            }
            throw new FormatException("Invalid Steam ID Format");
        }
        
        return FindPlayersByName(identifier);
    }
    
    public static Player FindPlayerBySteamId(ulong steamId)
    {
        var first = UnitRegistry.playerLookup.First(p => p.Value.SteamID == steamId);
        return first.Value;
    }

    public static List<Player> FindPlayersByName(string targetPlayer)
    {
        return UnitRegistry.playerLookup
            .Where(p => p.Value.PlayerName.IndexOf(targetPlayer, StringComparison.OrdinalIgnoreCase) >= 0)
            .Select(p => p.Value)
            .ToList();
    }
}
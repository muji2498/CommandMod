using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NuclearOption.Networking;

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
            return null;
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
        var regex = GlobToRegex(targetPlayer);
        
        return UnitRegistry.playerLookup
            .Where(p => Regex.IsMatch(p.Value.PlayerName, regex, RegexOptions.IgnoreCase))
            .Select(p => p.Value)
            .ToList();
    }
    
    // a special thanks to DownloadPizza for this suggestion. :)
    private static string GlobToRegex(string glob)
    {
        // Escape special regex characters in glob
        return "^" + Regex.Escape(glob)
                       .Replace(@"\*", ".*")
                       .Replace(@"\?", ".") + "$";
    }
}
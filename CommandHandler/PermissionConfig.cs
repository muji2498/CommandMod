using System.Collections.Generic;

namespace CommandMod.CommandHandler;

public class PermissionConfig
{
    public Dictionary<ulong, Roles> PlayerRoles { get; set; } = new();

    public bool HasRole(ulong steamId, Roles role)
    {
        var playerRoles = GetRole(steamId);
        Plugin.Logger.LogInfo($"Checking permissions for player {steamId} who has player roles:({playerRoles}) with role:({role})");
        return (playerRoles & role) != 0;
    }

    public Roles GetRole(ulong playerId)
    {
        lock (PlayerRoles)
        {
            if (PlayerRoles.TryGetValue(playerId, out var role))
            {
                return role;
            }
        }
        return Roles.None;
    }

    public void SetRole(ulong playerId, Roles role)
    {
        lock (PlayerRoles)
        {
            PlayerRoles[playerId] = role;
        }
        Plugin.Instance.PermissionConfigManager.SaveConfig();
    }

    public void AddRole(ulong playerId, Roles role)
    {
        SetRole(playerId, GetRole(playerId) | role);
    }

    public void RemoveRole(ulong playerId, Roles role)
    {
        SetRole(playerId, GetRole(playerId) & ~role);
    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandMod.CommandHandler;

public class PermissionConfig
{
    // From https://modules.battlebit.community/module/36
    
    public Dictionary<string, List<string>> Groups { get; set; } = new();
    public Dictionary<ulong, List<string>> PlayerGroups { get; set; } = new();
    public Dictionary<ulong, List<string>> PlayerPermissions { get; set; } = new();

    public bool HasPermission(ulong steamId,string permission)
    {
        Plugin.Logger.LogDebug($"Checking permission if player {steamId} has permission for {permission}");

        var playerPermission = this.GetAllPlayerPermissions(steamId);
        
        if (playerPermission.Contains("*"))
        {
            Plugin.Logger.LogDebug($"Player {steamId} has wildcard permission '*'.");
            return true;
        }

        if (playerPermission.Contains(permission))
        {
            Plugin.Logger.LogDebug($"Player {steamId} has explicit permission {permission}'.");
            return true;
        }

        foreach (var perm in playerPermission)
        {
            if (perm.EndsWith(".*") && permission.StartsWith(perm.TrimEnd('*')))
            {
                Plugin.Logger.LogDebug($"Player {steamId} has pattern-matching permission {perm} for {permission}'.");
                return true;
            }
        }

        if (playerPermission.Contains($"-{permission}"))
        {
            Plugin.Logger.LogDebug($"Player {steamId} has explicitly revoked permission for {permission}'.");
            return false;
        }

        foreach (var perm in playerPermission.Where(p => p.StartsWith("-") && p.EndsWith(".*")))
        {
            if (permission.StartsWith(perm.TrimStart('-').TrimEnd('*')))
            {
                Plugin.Logger.LogDebug($"player {steamId} has pattern-revoked permission {perm} for {permission}'.");
                return false;
            }
        }
        
        Plugin.Logger.LogDebug($"Player {steamId} has no permission for {permission}'.");
        return false;
    }
    

    public void AddGroup(string group)
    {
        Plugin.Logger.LogDebug($"Adding group {group}.");
        if (Groups.ContainsKey(group))
        {
            Plugin.Logger.LogDebug($"Group {group} already exists.");
            return;
        }
        Groups.Add(group, new());
    }
    
    
    public void RemoveGroup(string group)
    {
        Plugin.Logger.LogDebug($"Removing group {group}.");
        if (!Groups.ContainsKey(group))
        {
            Plugin.Logger.LogDebug($"Group {group} does not exist.");
            return;
        }
        Groups.Remove(group);
    }
    
    public string[] GetGroups() => Groups.Keys.ToArray();

    public string[] GetPlayerGroups(ulong steamId)
    {
        if (!PlayerGroups.ContainsKey(steamId))
        {
            if (Groups.ContainsKey("*"))
            {
                return new[] { "*" };
            }
            else
            {
                return Array.Empty<string>();
            }
        }   
        return PlayerGroups[steamId].ToArray();
    }

    public void AddGroupPermission(string group, string permission)
    {
        Plugin.Logger.LogDebug($"Adding permission {permission} to {group}.");

        if (!Groups.ContainsKey(group))
        {
            Plugin.Logger.LogDebug($"Group {group} does not exist.");
            return;
        }

        if (Groups[group].Contains(permission))
        {
            Plugin.Logger.LogDebug($"Group {group} already contains permission {permission}.");
            return;
        }
        
        Groups[group].Add(permission);
    }

    public void RemoveGroupPermission(string group, string permission)
    {
        Plugin.Logger.LogDebug($"Removing permission {permission} to {group}.");

        if (!Groups.ContainsKey(group))
        {
            Plugin.Logger.LogDebug($"Group {group} does not exist.");
            return;
        }

        if (!Groups[group].Contains(permission))
        {
            Plugin.Logger.LogDebug($"Group {group} does not contain permission {permission}.");
            return;
        }
        
        Groups[group].Remove(permission);
    }

    public void AddRevokedGroupPermission(string group, string permission)
    {
        Plugin.Logger.LogDebug($"Revoking permission {permission} to {group}.");
        
        this.AddGroupPermission(group, $"-{permission}");
    }
    
    public void RemoveRevokedGroupPermission(string group, string permission)
    {
        Plugin.Logger.LogDebug($"Revoking permission {permission} to {group}.");
        
        this.RemoveGroupPermission(group, $"-{permission}");
    }
    
    public string[] GetGroupPermissions(string group)
    {
        if (!Groups.ContainsKey(group))
        {
            Plugin.Logger.LogDebug($"Group {group} does not exist.");
            return Array.Empty<string>();
        }
        return Groups[group].ToArray();
    }

    public void AddPlayerGroup(ulong steamId, string group)
    {
        Plugin.Logger.LogDebug($"Adding player {steamId} to {group}.");

        if (!PlayerGroups.ContainsKey(steamId))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have any groups.");
            PlayerGroups.Add(steamId, new());
        }

        if (PlayerGroups[steamId].Contains(group))
        {
            Plugin.Logger.LogDebug($"Player {steamId} already has group {group}.");
            return;
        }
        
        PlayerGroups[steamId].Add(group);
    }

    public void RemovePlayerGroup(ulong steamId, string group)
    {
        Plugin.Logger.LogDebug($"Removing player {steamId} from {group}.");

        if (!PlayerGroups.ContainsKey(steamId))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have any groups.");
            return;
        }

        if (!PlayerGroups[steamId].Contains(group))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have group {group}.");
            return;
        }
        
        PlayerGroups[steamId].Remove(group);
    }

    public void AddPlayerPermission(ulong steamId, string permission)
    {
        Plugin.Logger.LogDebug($"Adding permission {permission} to {steamId}.");

        if (!PlayerPermissions.ContainsKey(steamId))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have any permission.");
            PlayerPermissions.Add(steamId, new());
        }

        if (PlayerPermissions[steamId].Contains(permission))
        {
            Plugin.Logger.LogDebug($"Player {steamId} already contains permission {permission}.");
            return;
        }
        
        PlayerPermissions[steamId].Add(permission);
    }

    public void RemovePlayerPermission(ulong steamId, string permission)
    {
        Plugin.Logger.LogDebug($"Removing permission {permission} to {steamId}.");

        if (!PlayerPermissions.ContainsKey(steamId))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have any permission.");
            return;
        }

        if (!PlayerPermissions[steamId].Contains(permission))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have permission {permission}.");
            return;
        }
        
        PlayerPermissions[steamId].Remove(permission);
    }

    public void AddRevokedPlayerPermission(ulong steamId, string permission)
    {
        Plugin.Logger.LogDebug($"Adding revoked permission {permission} to {steamId}.");
        this.AddPlayerPermission(steamId, $"-{permission}");
    }

    public void RemoveRevokedPlayerPermission(ulong steamId, string permission)
    {
        Plugin.Logger.LogDebug($"Removing revoked permission {permission} to {steamId}.");
        this.RemovePlayerPermission(steamId, $"-{permission}");
    }

    public string[] GetPlayerPermissions(ulong steamId)
    {
        if (!PlayerPermissions.ContainsKey(steamId))
        {
            Plugin.Logger.LogDebug($"Player {steamId} does not have any player-specific permissions.");
            return Array.Empty<string>();
        }
        
        return PlayerPermissions[steamId].ToArray();
    }

    public string[] GetAllPlayerPermissions(ulong steamId)
    {
        List<string> permissions = new();

        if (PlayerPermissions.ContainsKey(steamId))
        {
            permissions.AddRange(PlayerPermissions[steamId]);
        }

        foreach (var group in this.GetPlayerGroups(steamId))
        {
            permissions.AddRange(this.GetGroupPermissions(group));
        }
        
        return permissions.Distinct().ToArray();
    }
}
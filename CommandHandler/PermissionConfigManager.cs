using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Newtonsoft.Json;

namespace CommandMod.CommandHandler;

public class PermissionConfigManager
{
    private readonly string _configPath;
    public PermissionConfig Config { get; private set; }

    public PermissionConfigManager(BaseUnityPlugin plugin)
    {
        _configPath = Path.Combine(Paths.ConfigPath, "PermissionConfig.json");
        LoadOrCreateConfig();
    }

    private void LoadOrCreateConfig()
    {
        if (!File.Exists(_configPath))
        {
            Config = GenerateDefaultConfig();
            SaveConfig();
            Plugin.Logger.LogInfo("Default permission config loaded.");
        }
        else
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                Config = JsonConvert.DeserializeObject<PermissionConfig>(json);
                Plugin.Logger.LogInfo("permission config loaded.");
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"failed to load permission config: {e.Message}");
                Config = GenerateDefaultConfig();
                SaveConfig();
            }
        }
    }
    
    public void SaveConfig()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(_configPath, json);
            Plugin.Logger.LogInfo("Permissions config saved.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"Failed to save permissions config: {ex.Message}");
        }
    }

    private PermissionConfig GenerateDefaultConfig()
    {
        return new PermissionConfig
        {
            Groups = new Dictionary<string, List<string>>
            {
                ["*"] = new List<string> { "CommandHandler.*" },
                ["Owner"] = new List<string> { "*" },
                ["Admin"] = new List<string> { "ModeratorTools.*" },
                ["Moderator"] = new List<string>
                {
                    "ModeratorTools.Kick",
                    "ModeratorTools.Ban",
                    "ModeratorTools.Teleport"
                },
                ["VIP"] = new List<string> { "ChatOverwrite.Rainbow" },
                ["Chat Moderator"] = new List<string>
                {
                    "ModeratorTools.Say",
                    "ModeratorTools.SayToPlayer",
                    "ModeratorTools.Message",
                    "ModeratorTools.Clear",
                    "ModeratorTools.Gag",
                    "ModeratorTools.Ungag"
                },
                ["Revoked Detailed Help"] = new List<string> { "-CommandHandler.CommandHelp" },
                ["Revoked All"] = new List<string> { "-*" }
            },
            PlayerGroups = new Dictionary<ulong, List<string>>
            {
                [76561198142010443] = new List<string> { "Owner" },
                [12341234123412341] = new List<string> { "VIP", "Chat Moderator" }
            },
            PlayerPermissions = new Dictionary<ulong, List<string>>
            {
                [76561198142010443] = new List<string> { "*" }
            }
        };
    }
    
}
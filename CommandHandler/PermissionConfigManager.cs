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

    public PermissionConfigManager()
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
            PlayerRoles = new Dictionary<ulong, Roles>
            {
                [76561198181797231] = Roles.Admin | Roles.Moderator,
                [76561198181797631] = Roles.Owner,
                [76561198181797281] = Roles.Moderator,
                [76561198181797233] = Roles.None,
            }
        };
    }
    
}
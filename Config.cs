using BepInEx.Configuration;

namespace CommandMod;

public class Config(ConfigFile file)
{
    public ConfigFile File { get; } = file;
    
    // General
    public ConfigEntry<string> Prefix { get; } = file.Bind("Command", "Prefix", "!", "This is the prefix commands will use for example. (!list)");
}
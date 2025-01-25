using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace CommandMod.CommandHandler;

public static class CommandLoader
{
    public static void RegisterCommandsFromPlugins(ChatCommandHandler handler)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var thisAssembly = Assembly.GetExecutingAssembly();

        foreach (var assembly in assemblies)
        {
            if (ReferencesMe(assembly) || assembly == thisAssembly)
            {
                Plugin.Logger.LogInfo($"Registering Commands from {assembly.GetName().Name}");
                RegisterCommandsFromAssembly(assembly, handler);    
            }
        }
    }
    
    public static void RegisterCommandsFromAssembly(Assembly assembly, ChatCommandHandler handler)
    {
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            var methods =  AccessTools.GetDeclaredMethods(type).ToArray();
            foreach (var method in methods)
            {
                ConsoleCommandAttribute attribute = null;
                try
                {
                    attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
                    if (attribute == null) continue;
                }
                catch (Exception)
                {
                    Plugin.Logger.LogError($"Skipping. Invalid command attribute: {attribute} - {method.Name} in {assembly.GetName().Name} Please Update the command attribute first.");
                    continue;
                }
                
                var commandName = attribute.CommandName;
                    
                if (method.GetParameters().Length == 2 &&
                    method.GetParameters()[0].ParameterType == typeof(string[]) &&
                    method.GetParameters()[1].ParameterType == typeof(CommandObjects) &&
                    method.ReturnType == typeof(void))
                {
                    try
                    {
                        var action = (Action<string[], CommandObjects>)Delegate.CreateDelegate(
                            typeof(Action<string[], CommandObjects>), method);
                        
                        handler.RegisterCommand(type, commandName, action, attribute.Roles);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogError($"Failed to create delegate for command {commandName}, method needs to be static: {ex.Message}");
                    }
                }
                else
                {
                    Plugin.Logger.LogError($"Method {method.Name} in {type.FullName} does not match the required signature (string[], CommandObjects)");
                }
            }
        }
    }

    private static bool ReferencesMe(Assembly assembly)
    {
        try
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            return referencedAssemblies.Any(a => a.Name.StartsWith("CommandMod"));
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
using CommandMod.CommandHandler;
using HarmonyLib;
using UnityEngine.UI;

namespace CommandMod.Patches;

public class MainMenuPatch
{
    // [HarmonyPatch(typeof(MultiplayerMenu), "Start")]
    // static class StartPatches
    // {
    //     static bool Prefix(MultiplayerMenu __instance)
    //     {
    //         var maxPlayerSlider = AccessTools.Field(typeof(MultiplayerMenu), "maxPlayersSlider");
    //         var slider = (Slider)maxPlayerSlider.GetValue(__instance);
    //         slider.maxValue = 1024;
    //         slider.value = 512;
    //         
    //         return true;
    //     }
    // }
}
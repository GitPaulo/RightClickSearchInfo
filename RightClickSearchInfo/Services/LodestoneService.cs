using Dalamud.Utility;
using RightClickSearchInfo.Utils;
using System;

namespace RightClickSearchInfo.Services;

public class LodestoneService(Plugin plugin)
{
    public void OpenCharacterLodestone(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId, plugin);
        if (world == "Unknown")
        {
            Plugin.ChatGui.Print("Failed to retrieve world name. Opening lodestone without world information.");
        }
        var encodedWorld = Uri.EscapeDataString(world);
        var encodedFullName = Uri.EscapeDataString(fullName);
        Util.OpenLink($"https://eu.finalfantasyxiv.com/lodestone/character/?q={encodedFullName}&worldname={encodedWorld}");
        plugin.SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }
}

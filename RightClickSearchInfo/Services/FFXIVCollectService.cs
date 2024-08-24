using Dalamud.Utility;
using System;
using RightClickSearchInfo.Utils;

namespace RightClickSearchInfo.Services;

public class FFXIVCollectService(Plugin plugin)
{
    public void OpenCharacterFFXIVCollect(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId, plugin);
        if (world == "Unknown")
        {
            Plugin.ChatGui.Print("Failed to retrieve world name. Opening FFXIV collect without world information.");
        }
        var encodedWorld = Uri.EscapeDataString(world);
        var encodedFullName = Uri.EscapeDataString(fullName);
        Util.OpenLink($"https://ffxivcollect.com/characters/search?server={encodedWorld}&name={encodedFullName}");
        plugin.SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }
}

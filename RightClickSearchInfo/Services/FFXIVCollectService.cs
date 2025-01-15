using System;
using RightClickSearchInfo.Util;

namespace RightClickSearchInfo.Services;

public class FFXIVCollectService()
{
    public void OpenCharacterFFXIVCollect(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Shared.Chat.Print("Failed to retrieve world name. Opening FFXIV collect without world information.");
        }
        
        var encodedWorld = Uri.EscapeDataString(world);
        var encodedFullName = Uri.EscapeDataString(fullName);
        
        Dalamud.Utility.Util.OpenLink($"https://ffxivcollect.com/characters/search?server={encodedWorld}&name={encodedFullName}");
        Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
    }
}

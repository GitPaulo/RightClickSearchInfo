using System;
using RightClickSearchInfo.Util;

namespace RightClickSearchInfo.Services;

public class LodestoneService()
{
    public void OpenCharacterLodestone(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Shared.Chat.Print("Failed to retrieve world name. Opening lodestone without world information.");
        }
        
        var encodedWorld = Uri.EscapeDataString(world);
        var encodedFullName = Uri.EscapeDataString(fullName);
        
        Dalamud.Utility.Util.OpenLink($"https://eu.finalfantasyxiv.com/lodestone/character/?q={encodedFullName}&worldname={encodedWorld}");
        Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
    }
}

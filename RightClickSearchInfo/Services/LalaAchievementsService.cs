using RightClickSearchInfo.Util;

namespace RightClickSearchInfo.Services;

public class LalaAchievementsService
{
    public void OpenCharacterLalaAchievements(string fullName, uint worldId)
    {
        var world = WorldUtils.WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Shared.Chat.Print("Failed to retrieve world name. Opening lodestone without world information.");
        }
        
        var lodestoneId = Shared.LodestoneService.GetLodestoneId(fullName, world);
        Dalamud.Utility.Util.OpenLink($"https://lalachievements.com/char/{lodestoneId}/");
        Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
    }
}

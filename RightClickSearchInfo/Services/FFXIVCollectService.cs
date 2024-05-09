using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;

namespace RightClickSearchInfo.Services;

public class FFXIVCollectService(Plugin plugin)
{
    public void OpenCharacterFFXIVCollect(string fullName, uint worldId)
    {
        var world = WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Plugin.ChatGui.Print("Failed to retrieve world name. Opening FFXIV collect without world information.");
        }
        Util.OpenLink($"https://ffxivcollect.com/characters/search?server={world}&name={fullName}");
        plugin.SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }


    private string WorldIdToName(uint worldId)
    {
        var worlds = plugin.DataManager.GetExcelSheet<World>();
        var world = worlds?.GetRow(worldId);
        return world != null ? world.Name : "Unknown";
    }
}
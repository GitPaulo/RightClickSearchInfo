using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;

namespace RightClickSearchInfo.Services;

public class LodestoneService(Plugin plugin)
{
    public void OpenCharacterLodestone(string fullName, uint worldId)
    {
        var world = WorldIdToName(worldId);
        if (world == "Unknown")
        {
            Plugin.ChatGui.Print("Failed to retrieve world name. Opening lodestone without world information.");
        }
        Util.OpenLink($"https://eu.finalfantasyxiv.com/lodestone/character/?q={fullName}&worldname={world}");
        plugin.SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }


    private string WorldIdToName(uint worldId)
    {
        var worlds = plugin.DataManager.GetExcelSheet<World>();
        var world = worlds?.GetRow(worldId);
        return world != null ? world.Name : "Unknown";
    }
}

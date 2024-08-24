using Lumina.Excel.GeneratedSheets;

namespace RightClickSearchInfo.Utils
{
    public static class WorldUtils
    {
        public static string WorldIdToName(uint worldId, Plugin plugin)
        {
            var worlds = plugin.DataManager.GetExcelSheet<World>();
            var world = worlds?.GetRow(worldId);
            return world != null ? world.Name : "Unknown";
        }
    }
}

using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

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

        public static World WorldIdToWorld(uint worldId, Plugin plugin)
        {
            var worldSheet = plugin.DataManager.GetExcelSheet<World>()!;
            var world = worldSheet.FirstOrDefault(row => row.RowId == worldId);
            return world ?? worldSheet.First();
        }

        public static bool IsWorldValid(uint worldId, Plugin plugin)
        {
            return IsWorldValid(WorldIdToWorld(worldId, plugin));
        }

        public static bool IsWorldValid(World world)
        {
            if (world.Name.RawData.IsEmpty || GetRegionCode(world) == string.Empty)
            {
                return false;
            }

            return char.IsUpper((char)world.Name.RawData[0]);
        }

        public static string GetRegionCode(World world)
        {
            return world.DataCenter?.Value?.Region switch
            {
                1 => "JP",
                2 => "NA",
                3 => "EU",
                4 => "OC",
                _ => string.Empty,
            };
        }
    }
}

using System.Linq;
using Lumina.Excel.Sheets;

namespace RightClickSearchInfo.Util
{
    public static class WorldUtils
    {
        public static string WorldIdToName(uint worldId)
        {
            var worlds = Shared.DataManager.GetExcelSheet<World>();
            var world = worlds?.GetRow(worldId);
            
            return world != null ? world.Value.Name.ToString() : "Unknown";
        }

        public static World WorldIdToWorld(uint worldId)
        {
            var worldSheet = Shared.DataManager.GetExcelSheet<World>()!;
            var world = worldSheet.FirstOrDefault(row => row.RowId == worldId);
            
            return world;
        }

        public static bool IsWorldValid(uint worldId)
        {
            return IsWorldValid(WorldIdToWorld(worldId));
        }

        public static bool IsWorldValid(World world)
        {
            return !world.Name.Data.IsEmpty && GetRegionCode(world) != string.Empty;
        }

        public static string GetRegionCode(World world)
        {
            return world.DataCenter.Value.Region switch
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

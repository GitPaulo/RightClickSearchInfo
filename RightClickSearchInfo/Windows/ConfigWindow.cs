using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RightClickSearchInfo.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("RightClickSearchInfo###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 200);
        SizeCondition = ImGuiCond.Always;
    }

    public void Dispose() { }


    public override void Draw()
    {
        var shouldConfigurationShowSearchInfoItem = Shared.Config.ShowSearchInfoItem;
        if (ImGui.Checkbox("Show search info item?", ref shouldConfigurationShowSearchInfoItem))
        {
            Shared.Config.ShowSearchInfoItem = shouldConfigurationShowSearchInfoItem;
            Shared.Config.Save();
        }
        {}
        var shouldConfigurationShowFFLogsItem = Shared.Config.ShowFFLogsItem;
        if (ImGui.Checkbox("Show FFlogs item?", ref shouldConfigurationShowFFLogsItem))
        {
            Shared.Config.ShowFFLogsItem = shouldConfigurationShowFFLogsItem;
            Shared.Config.Save();
        }

        var shouldShowFFXIVCollectItem = Shared.Config.ShowFFXIVCollectItem;
        if (ImGui.Checkbox("Show FFXIV Collect item?", ref shouldShowFFXIVCollectItem))
        {
            Shared.Config.ShowFFXIVCollectItem = shouldShowFFXIVCollectItem;
            Shared.Config.Save();
        }

        var shouldShowLodestoneItem = Shared.Config.ShowLodestoneItem;
        if (ImGui.Checkbox("Show Lodestone item?", ref shouldConfigurationShowFFLogsItem))
        {
            Shared.Config.ShowLodestoneItem = shouldShowLodestoneItem;
            Shared.Config.Save();
        }
        
        var shouldShowLalaAchievementsItem = Shared.Config.ShowLalaAchievementsItem;
        if (ImGui.Checkbox("Show Lala Achievements item?", ref shouldShowLalaAchievementsItem))
        {
            Shared.Config.ShowLalaAchievementsItem = shouldShowLalaAchievementsItem;
            Shared.Config.Save();
        }
    }
}

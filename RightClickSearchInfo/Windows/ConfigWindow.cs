using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RightClickSearchInfo.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private Plugin _plugin;

    public ConfigWindow(Plugin plugin) : base("RightClickSearchInfo###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 200);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
        _plugin = plugin;
    }

    public void Dispose() { }


    public override void Draw()
    {
        var shouldConfigurationShowSearchInfoItem = Configuration.ShowSearchInfoItem;
        if (ImGui.Checkbox("Show search info item?", ref shouldConfigurationShowSearchInfoItem))
        {
            Configuration.ShowSearchInfoItem = shouldConfigurationShowSearchInfoItem;
            Configuration.Save(_plugin);
        }
        {}
        var shouldConfigurationShowFFLogsItem = Configuration.ShowFFLogsItem;
        if (ImGui.Checkbox("Show FFlogs item?", ref shouldConfigurationShowFFLogsItem))
        {
            Configuration.ShowFFLogsItem = shouldConfigurationShowFFLogsItem;
            Configuration.Save(_plugin);
        }

        var shouldShowFFXIVCollectItem = Configuration.ShowFFXIVCollectItem;
        if (ImGui.Checkbox("Show FFXIV Collect item?", ref shouldShowFFXIVCollectItem))
        {
            Configuration.ShowFFXIVCollectItem = shouldShowFFXIVCollectItem;
            Configuration.Save(_plugin);
        }

        var shouldShowLodestoneItem = Configuration.ShowLodestoneItem;
        if (ImGui.Checkbox("Show Lodestone item?", ref shouldConfigurationShowFFLogsItem))
        {
            Configuration.ShowLodestoneItem = shouldShowLodestoneItem;
            Configuration.Save(_plugin);
        }
    }
}

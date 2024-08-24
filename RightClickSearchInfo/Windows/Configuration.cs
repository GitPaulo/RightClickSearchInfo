using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace RightClickSearchInfo.Windows;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool ShowLodestoneItem { get; set; } = true;
    public bool ShowFFXIVCollectItem { get; set; } = true;
    public bool ShowSearchInfoItem { get; set; } = true;
    public bool ShowFFLogsItem { get; set; } = true;

    // the below exist just to make saving less cumbersome
    public void Save(Plugin plugin)
    {
        plugin.PluginInterface.SavePluginConfig(this);
    }
}

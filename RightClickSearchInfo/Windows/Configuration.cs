using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace RightClickSearchInfo.Windows;

[Serializable]
public class CustomSearchProvider
{
    // Display name of the menu item (e.g. "In RP Finder")
    public string Label = "New Search";

    // URL with placeholders: $1 for full name, $2 for world
    public string UrlTemplate = "https://example.com/search?name=$1&world=$2";
}

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool ShowLodestoneItem { get; set; } = true;
    public bool ShowFFXIVCollectItem { get; set; } = true;
    public bool ShowSearchInfoItem { get; set; } = true;
    public bool ShowFFLogsItem { get; set; } = true;
    public bool ShowLalaAchievementsItem { get; set; } = true;

    public List<CustomSearchProvider> CustomSearchProviders { get; set; } = new();

    // Save using Dalamud's built-in config handling
    public void Save()
    {
        Shared.PluginInterface.SavePluginConfig(this);
    }
}

using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RightClickSearchInfo.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("RightClickSearchInfo Configuration###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoCollapse;

        Size = new Vector2(600, 300);
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

        { }
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

        // Custom Searches
        ImGui.Separator();
        ImGui.Text("Custom Search Items");
        ImGui.Text("(For template URLs, use $1, $2, ... as a placeholder for query parameter values.)");
        ImGui.Text("(e.g. https://example.com/search?name=$1&world=$2)");
        ImGui.SameLine();
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("URL Placeholders:");
            ImGui.Separator();
            ImGui.TextUnformatted("$1 → Character full name");
            ImGui.TextUnformatted("$2 → World name");
            ImGui.TextUnformatted("$3 → Lodestone ID");
            ImGui.TextUnformatted("$first → First name");
            ImGui.TextUnformatted("$last → Last name");
            ImGui.EndTooltip();
        }

        if (ImGui.BeginTable("##CustomSearchTable", 3,
                             ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("URL Template", ImGuiTableColumnFlags.WidthStretch, 0.7f);
            ImGui.TableSetupColumn("Delete", ImGuiTableColumnFlags.WidthFixed, 60f);
            ImGui.TableHeadersRow();

            int indexToRemove = -1;
            for (int i = 0; i < Shared.Config.CustomSearchProviders.Count; i++)
            {
                var provider = Shared.Config.CustomSearchProviders[i];
                ImGui.TableNextRow();

                // Label column
                ImGui.TableSetColumnIndex(0);
                ImGui.PushID($"label_{i}");
                ImGui.PushItemWidth(-1); // fill cell
                ImGui.InputText("", ref provider.Label, 100);
                ImGui.PopItemWidth();
                ImGui.PopID();

                // URL Template column
                ImGui.TableSetColumnIndex(1);
                ImGui.PushID($"url_{i}");
                ImGui.PushItemWidth(-1); // fill cell
                ImGui.InputText("", ref provider.UrlTemplate, 300);
                ImGui.PopItemWidth();
                ImGui.PopID();

                // Delete button column
                ImGui.TableSetColumnIndex(2);
                ImGui.PushID($"remove_{i}");
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.SmallButton(FontAwesomeIcon.Trash.ToIconString()))
                {
                    indexToRemove = i;
                }

                ImGui.PopFont();
                ImGui.PopID();
            }

            if (indexToRemove != -1)
            {
                Shared.Config.CustomSearchProviders.RemoveAt(indexToRemove);
                Shared.Config.Save();
            }

            ImGui.EndTable();
        }

        if (ImGui.Button("+ Add Search Item"))
        {
            Shared.Config.CustomSearchProviders.Add(new CustomSearchProvider());
            Shared.Config.Save();
        }
    }
}

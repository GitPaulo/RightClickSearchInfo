using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RightClickSearchInfo.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow() : base(
        "RightClickSearchInfo", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(360, 160),
        };
    }

    public void Dispose()
    {
        // TODO
    }

    public override void Draw()
    {
        ImGui.Text("You should right click someone to like search info.");
        ImGui.Text("Config menu will help you <3");
        ImGui.Spacing();
    }
}

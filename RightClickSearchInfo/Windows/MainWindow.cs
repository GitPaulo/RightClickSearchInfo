using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace RightClickSearchInfo.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow() : base(
        "RCSI", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose()
    {
        // TODO
    }

    public override void Draw()
    {
        ImGui.Text("You should right click someone to like search info.");
        ImGui.Text("IDK what you expected to be here?");
        ImGui.Spacing();
    }
}

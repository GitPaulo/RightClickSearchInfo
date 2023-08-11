using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace RightClickSearchInfo.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly TextureWrap GoatImage;
    private readonly Plugin Plugin;

    public MainWindow(Plugin plugin) : base(
        "RCSI", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
        GoatImage = plugin.PluginInterface.UiBuilder.LoadImage(Plugin.PluginResources.GoatPath);
    }

    public void Dispose()
    {
        GoatImage.Dispose();
    }

    public override void Draw()
    {
        ImGui.Text("You should right click someone to like search info.");

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Image(GoatImage.ImGuiHandle, new Vector2(GoatImage.Width, GoatImage.Height));
        ImGui.Unindent(55);
    }
}

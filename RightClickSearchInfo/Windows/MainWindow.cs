using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace RightClickSearchInfo.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly TextureWrap goatImage;
    private readonly Plugin plugin;

    public MainWindow(Plugin plugin) : base(
        "RCSI", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        goatImage = plugin.PluginInterface.UiBuilder.LoadImage(this.plugin.PluginResources.GoatPath);
    }

    public void Dispose()
    {
        goatImage.Dispose();
    }

    public override void Draw()
    {
        ImGui.Text("You should right click someone to like search info.");

        ImGui.Spacing();

        ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Image(goatImage.ImGuiHandle, new Vector2(goatImage.Width, goatImage.Height));
        ImGui.Unindent(55);
    }
}

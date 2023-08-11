using ImGuiNET;
using RightClickSearchInfo.Sound;

namespace RightClickSearchInfo.Services;

public class SearchCommandService
{
    private readonly Plugin plugin;

    public SearchCommandService(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public void GenerateToClipboard(string fullName)
    {
        var commandStr = CommandFromFullName(fullName);

        // TODO: Not possible i think? so we copy to clipboard and play a sound lol
        Plugin.ChatGui.Print("Copied search command to clipboard:");
        Plugin.ChatGui.Print(commandStr);

        ImGui.SetClipboardText(commandStr);

        SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }

    private string CommandFromFullName(string fullName)
    {
        var targetNameSplit = fullName.Split(' ');
        return "/search forename \"" + targetNameSplit[0] + "\" surname \"" + targetNameSplit[1] + "\"";
    }
}

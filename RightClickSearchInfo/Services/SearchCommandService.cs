using ImGuiNET;

using RightClickSearchInfo.Sound;

namespace RightClickSearchInfo.Services;

// Very object object poop
// ngl i thought this was going to have more stuff in it initial lmao
public class SearchCommandService
{
  private readonly Plugin plugin;
  private readonly string notifPath;

  public SearchCommandService(Plugin plugin, string notifPath)
  {
    this.plugin = plugin;
    this.notifPath = notifPath;
  }

  public void runSearch(string fullName)
  {
    string commandStr = this.CommandFromFullName(fullName);

    // TODO: Not possible i think? so we copy to clipboard and play a sound lol
    Plugin.ChatGui.Print("Copied search command to clipboard:");
    Plugin.ChatGui.Print(commandStr);

    ImGui.SetClipboardText(commandStr);

    SoundEngine.PlaySound(this.notifPath);
  }

  private string CommandFromFullName(string fullName)
  {
    string[] targetNameSplit = fullName.Split(' ');
    return "/search forename \"" + targetNameSplit[0] + "\" surname \"" + targetNameSplit[1] + "\"";
  }
}

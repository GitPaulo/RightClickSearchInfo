namespace RightClickSearchInfo.ContextMenus;

using Dalamud.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

using ImGuiNET;

public class TargetContextMenu
{
    private readonly Plugin plugin;
    private readonly GameObjectContextMenuItem addShowMenuItem;

    private string targetName;

    public TargetContextMenu(Plugin plugin)
    {
        this.plugin = plugin;
        this.plugin.ContextMenu.OnOpenGameObjectContextMenu += this.OpenGameObjectContextMenu;
        this.addShowMenuItem = new GameObjectContextMenuItem(
            new SeString(new TextPayload("View In Search")), this.OnOpenPlayerInfo);
        this.targetName = "";
    }

    public void Dispose()
    {
        this.plugin.ContextMenu.OnOpenGameObjectContextMenu -= this.OpenGameObjectContextMenu;
    }

    private static bool shouldShowMenu(BaseContextMenuArgs args)
    {
        switch (args.ParentAddonName)
        {
            case null:
            case "LookingForGroup":
            case "PartyMemberList":
            case "FriendList":
            case "FreeCompany":
            case "SocialList":
            case "ContactList":
            case "ChatLog":
            case "_PartyList":
            case "LinkShell":
            case "CrossWorldLinkshell":
            case "ContentMemberList":
            case "BlackList":
                return args.Text != null && args.ObjectWorld != 0 && args.ObjectWorld != 65535;

            default:
                return false;
        }
    }

    private void OpenGameObjectContextMenu(GameObjectContextMenuOpenArgs args)
    {
        // hide on own player
        if (args.ObjectId == Plugin.ClientState.LocalPlayer!.ObjectId) return;

        // validate menu
        if (!shouldShowMenu(args)) return;

        // save target name
        this.targetName = args.Text!.ToString();

        args.AddCustomItem(this.addShowMenuItem);
    }

    private void OnOpenPlayerInfo(GameObjectContextMenuItemSelectedArgs args)
    {
        string[] targetNameSplit = this.targetName.Split(' ');
        string searchCommandString = "/search forename \"" + targetNameSplit[0] + "\" surname \"" + targetNameSplit[1] + "\"";
        Plugin.ChatGui.Print("Copied search command to clipboard:");
        Plugin.ChatGui.Print(searchCommandString);
        ImGui.SetClipboardText(searchCommandString);
    }
}

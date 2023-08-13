using Dalamud.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace RightClickSearchInfo.ContextMenus;

public class TargetContextMenu
{
    private readonly GameObjectContextMenuItem lodestoneItem;
    private readonly Plugin plugin;
    private readonly GameObjectContextMenuItem searchItem;

    private string? targetFullName;

    public TargetContextMenu(Plugin plugin)
    {
        this.plugin = plugin;
        this.plugin.ContextMenu.OnOpenGameObjectContextMenu += OpenGameObjectContextMenu;
        searchItem = new GameObjectContextMenuItem(
            new SeString(new TextPayload("View In Search")), OnOpenPlayerInfo);
        lodestoneItem = new GameObjectContextMenuItem(
            new SeString(new TextPayload("Open In Lodestone")), OnOpenLodestone);
        targetFullName = null;
    }

    public void Dispose()
    {
        plugin.ContextMenu.OnOpenGameObjectContextMenu -= OpenGameObjectContextMenu;
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
        targetFullName = args.Text!.ToString();

        // add item to context menu
        args.AddCustomItem(searchItem);
        args.AddCustomItem(lodestoneItem);
    }

    private void OnOpenPlayerInfo(GameObjectContextMenuItemSelectedArgs args)
    {
        if (targetFullName == null) return;

        var targetNameSplit = targetFullName.Split(' ');
        var searchCommand = $"/search forename \"{targetNameSplit[0]}\" surname \"{targetNameSplit[1]}\"";

        #pragma warning disable CS4014
        plugin.ChatAutomationService.SendMessage(searchCommand);
    }

    private void OnOpenLodestone(GameObjectContextMenuItemSelectedArgs args)
    {
        if (targetFullName == null) return;

        plugin.LodestoneService.OpenCharacterLodestone(targetFullName, args.ObjectWorld);
    }
}

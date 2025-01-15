using System.Linq;
using Dalamud.Game.Gui.ContextMenu;
using RightClickSearchInfo.Util;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu
    {
        private readonly MenuItem searchMenuItem;
        private readonly MenuItem ffLogsMenuItem;
        private readonly MenuItem lodestoneMenuItem;
        private readonly MenuItem ffxivCollectMenuItem;

        private string? targetFullName;
        private uint targetWorldId;

        public TargetContextMenu()
        {
            searchMenuItem = new MenuItem
            {
                Name = "Search In Game",
                OnClicked = OnOpenPlayerInfo,
                PrefixChar = 'S'
            };
            ffLogsMenuItem = new MenuItem
            {
                Name = "Search In FFLogs",
                OnClicked = OnOpenFFLogs,
                PrefixChar = 'S'
            };
            lodestoneMenuItem = new MenuItem
            {
                Name = "Search In Lodestone",
                OnClicked = OnOpenLodestone,
                PrefixChar = 'S'
            };
            ffxivCollectMenuItem = new MenuItem
            {
                Name = "Search In FFXIV Collect",
                OnClicked = OnOpenFFXIVCollect,
                PrefixChar = 'S'
            };
        }

        public void Enable()
        {
            Shared.ContextMenu.OnMenuOpened += OnContextMenuOpened;
        }

        public void Disable()
        {
            Shared.ContextMenu.OnMenuOpened -= OnContextMenuOpened;
        }

        private static bool IsMenuValid(IMenuArgs menuOpenedArgs)
        {
            if (menuOpenedArgs.Target is not MenuTargetDefault menuTargetDefault)
            {
                return false;
            }

            switch (menuOpenedArgs.AddonName)
            {
                case null:
                case "LookingForGroup":
                case "PartyMemberList":
                case "FriendList":
                case "FreeCompany":
                case "SocialList":
                case "ContactList":
                case "ChatLog":
                case "PartyList":
                case "LinkShell":
                case "CrossWorldLinkshell":
                case "ContentMemberList":
                case "BeginnerChatList":
                    return menuTargetDefault.TargetName != string.Empty &&
                           WorldUtils.IsWorldValid(menuTargetDefault.TargetHomeWorld.RowId);

                case "BlackList":
                case "MuteList":
                    return menuTargetDefault.TargetName != string.Empty;
            }

            return false;
        }

        private void OnContextMenuOpened(IMenuOpenedArgs menuArgs)
        {
            if (!IsMenuValid(menuArgs) || menuArgs.Target is not MenuTargetDefault menuTargetDefault ||
                !Shared.PluginInterface.UiBuilder.ShouldModifyUi)
            {
                return;
            }

            targetFullName = menuTargetDefault.TargetName;
            targetWorldId = menuTargetDefault.TargetHomeWorld.RowId;

            if (Shared.Config.ShowSearchInfoItem)
            {
                menuArgs.AddMenuItem(searchMenuItem);
            }

            if (Shared.Config.ShowLodestoneItem)
            {
                menuArgs.AddMenuItem(lodestoneMenuItem);
            }

            if (Shared.Config.ShowFFXIVCollectItem)
            {
                menuArgs.AddMenuItem(ffxivCollectMenuItem);
            }

            // Check if FFLogs Shared is enabled
            var isFFLogsEnabled =
                Shared.PluginInterface.InstalledPlugins.Any(pluginInfo =>
                                                                pluginInfo.InternalName == "FFLogsViewer" &&
                                                                pluginInfo.IsLoaded) && Shared.Config.ShowFFLogsItem;
            if (!isFFLogsEnabled)
            {
                menuArgs.AddMenuItem(ffLogsMenuItem);
            }
        }

        private void OnOpenPlayerInfo(IMenuItemClickedArgs args)
        {
            if (targetFullName == null)
            {
                return;
            }

            var searchCommand = Shared.SearchInfoCommandService.CreateCommandString(targetFullName);
            _ = Shared.ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnOpenFFLogs(IMenuItemClickedArgs args)
        {
            if (targetFullName == null)
            {
                return;
            }

            Shared.FFLogsService.OpenCharacterFFLogs(targetFullName);
        }

        private void OnOpenLodestone(IMenuItemClickedArgs args)
        {
            if (targetFullName == null)
            {
                return;
            }

            Shared.LodestoneService.OpenCharacterLodestone(targetFullName, targetWorldId);
        }

        private void OnOpenFFXIVCollect(IMenuItemClickedArgs args)
        {
            if (targetFullName == null)
            {
                return;
            }

            Shared.FFXIVCollectService.OpenCharacterFFXIVCollect(targetFullName, targetWorldId);
        }
    }
}

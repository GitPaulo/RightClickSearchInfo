using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Utility;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu
    {
        private readonly Plugin _plugin;
        private readonly MenuItem _searchMenuItem;
        private readonly MenuItem _ffLogsMenuItem;
        private readonly MenuItem _lodestoneMenuItem;
        private readonly MenuItem _ffxivCollectMenuItem;

        private string? _targetFullName;
        private uint _targetWorldId;

        public TargetContextMenu(Plugin plugin)
        {
            _plugin = plugin;

            _searchMenuItem = new MenuItem
            {
                Name = "Search In Game",
                OnClicked = OnOpenPlayerInfo,
                PrefixChar = 'S'
            };
            _ffLogsMenuItem = new MenuItem
            {
                Name = "Search In FFLogs",
                OnClicked = OnOpenFFLogs,
                PrefixChar = 'S'
            };
            _lodestoneMenuItem = new MenuItem
            {
                Name = "Search In Lodestone",
                OnClicked = OnOpenLodestone,
                PrefixChar = 'S'
            };
            _ffxivCollectMenuItem = new MenuItem
            {
                Name = "Search In FFXIV Collect",
                OnClicked = OnOpenFFXIVCollect,
                PrefixChar = 'S'
            };
        }

        public void Enable()
        {
            _plugin.ContextMenu.OnMenuOpened += OnContextMenuOpened;
        }

        public void Disable()
        {
            _plugin.ContextMenu.OnMenuOpened -= OnContextMenuOpened;
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
                case "_PartyList":
                case "LinkShell":
                case "CrossWorldLinkshell":
                case "ContentMemberList":
                case "BeginnerChatList":
                    return menuTargetDefault.TargetName != string.Empty;

                case "BlackList":
                case "MuteList":
                    return menuTargetDefault.TargetName != string.Empty;
            }

            return false;
        }


        private void OnContextMenuOpened(IMenuOpenedArgs menuArgs)
        {
            if (!IsMenuValid(menuArgs) || menuArgs.Target is not MenuTargetDefault menuTargetDefault)
            {
                return;
            }

            _targetFullName = menuTargetDefault.TargetName;
            _targetWorldId = menuTargetDefault.TargetHomeWorld.Id;

            menuArgs.AddMenuItem(_searchMenuItem);
            menuArgs.AddMenuItem(_lodestoneMenuItem);
            menuArgs.AddMenuItem(_ffxivCollectMenuItem);

            // Check if FFLogs plugin is enabled
            var isFFLogsEnabled = _plugin.PluginInterface.InstalledPlugins.Any(pluginInfo => pluginInfo.InternalName == "FFLogsViewer" && pluginInfo.IsLoaded);
            if (!isFFLogsEnabled)
            {
                menuArgs.AddMenuItem(_ffLogsMenuItem);
            }
        }

        private void OnOpenPlayerInfo(IMenuItemClickedArgs args)
        {
            if (_targetFullName == null)
            {
                return;
            }

            var searchCommand = _plugin.SearchInfoCommandService.CreateCommandString(_targetFullName);
            _plugin.ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnOpenFFLogs(IMenuItemClickedArgs args)
        {
            if (_targetFullName == null)
            {
                return;
            }

           // TODO: Open FFLogs
        }

        private void OnOpenLodestone(IMenuItemClickedArgs args)
        {
            if (_targetFullName == null)
            {
                return;
            }

            _plugin.LodestoneService.OpenCharacterLodestone(_targetFullName, _targetWorldId);
        }

        private void OnOpenFFXIVCollect(IMenuItemClickedArgs args)
        {
            if (_targetFullName == null)
            {
                return;
            }

            _plugin.FFXIVCollectService.OpenCharacterFFXIVCollect(_targetFullName, _targetWorldId);
        }
    }
}

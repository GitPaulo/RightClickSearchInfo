using System.Linq;
using Dalamud.Game.Gui.ContextMenu;
using RightClickSearchInfo.Utils;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu
    {
        private Plugin _plugin;
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

        private static bool IsMenuValid(IMenuArgs menuOpenedArgs, Plugin plugin)
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
                    return menuTargetDefault.TargetName != string.Empty && WorldUtils.IsWorldValid(menuTargetDefault.TargetHomeWorld.Id, plugin);

                case "BlackList":
                case "MuteList":
                    return menuTargetDefault.TargetName != string.Empty;
            }

            return false;
        }

        private void OnContextMenuOpened(IMenuOpenedArgs menuArgs)
        {
            if (!IsMenuValid(menuArgs, _plugin) || menuArgs.Target is not MenuTargetDefault menuTargetDefault ||
                !_plugin.PluginInterface.UiBuilder.ShouldModifyUi)
            {
                return;
            }

            _targetFullName = menuTargetDefault.TargetName;
            _targetWorldId = menuTargetDefault.TargetHomeWorld.Id;

            if (_plugin.Configuration.ShowSearchInfoItem) {
                menuArgs.AddMenuItem(_searchMenuItem);
            }

            if (_plugin.Configuration.ShowLodestoneItem)
            {
                menuArgs.AddMenuItem(_lodestoneMenuItem);
            }

            if (_plugin.Configuration.ShowFFXIVCollectItem)
            {
                menuArgs.AddMenuItem(_ffxivCollectMenuItem);
            }

            // Check if FFLogs plugin is enabled
            var isFFLogsEnabled = _plugin.PluginInterface.InstalledPlugins.Any(pluginInfo => pluginInfo.InternalName == "FFLogsViewer" && pluginInfo.IsLoaded) && _plugin.Configuration.ShowFFLogsItem;
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

            _plugin.FFLogsService.OpenCharacterFFLogs(_targetFullName);
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

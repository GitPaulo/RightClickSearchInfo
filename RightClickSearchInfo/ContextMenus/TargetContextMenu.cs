using System;
using System.Collections.Generic;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu : IDisposable
    {
        private readonly Plugin _plugin;
        private readonly MenuItem _lodestoneMenuItem;
        private readonly MenuItem _searchMenuItem;
        private readonly MenuItem _ffxivCollectMenuItem;
        private string? _targetFullName;
        private uint _targetWorldId;

        private static IContextMenu? _contextMenu;

        public TargetContextMenu(Plugin plugin)
        {
            _plugin = plugin;
            _contextMenu = plugin.ContextMenu;

            _searchMenuItem = new MenuItem
            {
                Name = "🔍 View In Search",
                OnClicked = OnOpenPlayerInfo
            };
            _lodestoneMenuItem = new MenuItem
            {
                Name = "🌐 Search In Lodestone",
                OnClicked = OnOpenLodestone
            };
            _ffxivCollectMenuItem = new MenuItem
            {
                Name = "📘 Search In FFXIV Collect",
                OnClicked = OnOpenFFXIVCollect
            };

            Enable();
        }

        public void Dispose()
        {
            Disable();
        }

        private void Enable()
        {
            _contextMenu!.OnMenuOpened += OnContextMenuOpened;
        }

        private void Disable()
        {
            _contextMenu!.OnMenuOpened -= OnContextMenuOpened;
        }

        private bool IsMenuValid(IMenuArgs menuArgs)
        {
            if (menuArgs.Target is not MenuTargetDefault menuTargetDefault)
            {
                return false;
            }

            var validParentAddons = new HashSet<string>
            {
                "LookingForGroup", "PartyMemberList", "FriendList", "FreeCompany",
                "SocialList", "ContactList", "ChatLog", "_PartyList", "LinkShell",
                "CrossWorldLinkshell", "ContentMemberList", "BlackList"
            };

            return validParentAddons.Contains(menuArgs.AddonName) &&
                   !string.IsNullOrEmpty(menuTargetDefault.TargetName);
            // TODO: Util.IsWorldValid(menuTargetDefault.TargetHomeWorld.Id)
        }

        private void OnContextMenuOpened(IMenuOpenedArgs menuArgs)
        {
            if (!IsMenuValid(menuArgs))
            {
                return;
            }

            if (menuArgs.Target is not MenuTargetDefault menuTargetDefault)
            {
                return;
            }

            _targetFullName = menuTargetDefault.TargetName;
            _targetWorldId = menuTargetDefault.TargetHomeWorld.Id;

            menuArgs.AddMenuItem(_searchMenuItem);
            menuArgs.AddMenuItem(_lodestoneMenuItem);
            menuArgs.AddMenuItem(_ffxivCollectMenuItem);
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

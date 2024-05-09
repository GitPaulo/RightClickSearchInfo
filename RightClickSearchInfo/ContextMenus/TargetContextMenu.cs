using Dalamud.ContextMenu;
using System;
using System.Collections.Generic;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu : IDisposable
    {
        private readonly Plugin _plugin;
        private readonly GameObjectContextMenuItem _lodestoneMenuItem;
        private readonly GameObjectContextMenuItem _searchMenuItem;
        private readonly GameObjectContextMenuItem _ffxivCollectMenuItem;
        private string? _targetFullName;
        private ushort _targetWorldId;
        
        private static DalamudContextMenu? _contextMenu;

        public TargetContextMenu(Plugin plugin)
        {
            _plugin = plugin;
            _contextMenu = new DalamudContextMenu(_plugin.PluginInterface);
            
            _searchMenuItem = new GameObjectContextMenuItem("🔍 View In Search", OnOpenPlayerInfo);
            _lodestoneMenuItem = new GameObjectContextMenuItem("🌐 Search In Lodestone", OnOpenLodestone);
            _ffxivCollectMenuItem = new GameObjectContextMenuItem("📘 Search In FFXIV Collect", OnOpenFFXIVCollect);
            
            Enable();
        }

        public void Dispose()
        {
            Disable();
        }
        
        private void Enable()
        {
            _contextMenu!.OnOpenGameObjectContextMenu += OnOpenGameObjectContextMenu;
        }

        private void Disable()
        {
            _contextMenu!.OnOpenGameObjectContextMenu -= OnOpenGameObjectContextMenu;
        }

        private bool ShouldShowMenu(BaseContextMenuArgs args)
        {
            if (args.ParentAddonName == null) return true;

            var validParentAddons = new HashSet<string>
            {
                "LookingForGroup", "PartyMemberList", "FriendList", "FreeCompany",
                "SocialList", "ContactList", "ChatLog", "_PartyList", "LinkShell",
                "CrossWorldLinkshell", "ContentMemberList", "BlackList"
            };

            return validParentAddons.Contains(args.ParentAddonName) &&
                   args.Text != null &&
                   args.ObjectWorld != 0 &&
                   args.ObjectWorld != 65535;
        }

        private void OnOpenGameObjectContextMenu(GameObjectContextMenuOpenArgs args)
        {
            // Hide menu if it's the local player
            if (args.ObjectId == Plugin.ClientState?.LocalPlayer!.ObjectId)
            {
                return;
            }

            // Validate menu
            if (!ShouldShowMenu(args))
            {
                return;
            }

            // Save target name
            _targetFullName = args.Text!.ToString();
            _targetWorldId = args.ObjectWorld;

            // Add item to context menu
            args.AddCustomItem(_searchMenuItem);
            args.AddCustomItem(_lodestoneMenuItem);
            args.AddCustomItem(_ffxivCollectMenuItem);
        }

        private void OnOpenPlayerInfo(GameObjectContextMenuItemSelectedArgs args)
        {
            // If the target name is null, return
            if (_targetFullName == null)
            {
                return;
            }

            var searchCommand = _plugin.SearchInfoCommandService.CreateCommandString(_targetFullName);
            _ = _plugin.ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnOpenLodestone(GameObjectContextMenuItemSelectedArgs args)
        {
            // If the target name is null, return
            if (_targetFullName == null)
            {
                return;
            }

            _plugin.LodestoneService.OpenCharacterLodestone(_targetFullName, _targetWorldId);
        }
        
        private void OnOpenFFXIVCollect(GameObjectContextMenuItemSelectedArgs args)
        {
            // If the target name is null, return
            if (_targetFullName == null)
            {
                return;
            }

            _plugin.FFXIVCollectService.OpenCharacterFFXIVCollect(_targetFullName, _targetWorldId);
        }
    }}

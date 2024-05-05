using Dalamud.ContextMenu;
using System;
using System.Collections.Generic;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu : IDisposable
    {
        private readonly GameObjectContextMenuItem _lodestoneMenuItem;
        private readonly Plugin _plugin;
        private readonly GameObjectContextMenuItem _searchMenuItem;
        private string? _targetFullName;
        
        private static DalamudContextMenu? _contextMenu;

        public TargetContextMenu(Plugin plugin)
        {
            _plugin = plugin;
            _contextMenu = new DalamudContextMenu(_plugin.PluginInterface);
            
            _searchMenuItem = new GameObjectContextMenuItem("View In Search", OnOpenPlayerInfo);
            _lodestoneMenuItem = new GameObjectContextMenuItem("Search In Lodestone", OnOpenLodestone);
            
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
            if (args.ObjectId == Plugin.ClientState.LocalPlayer!.ObjectId)
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

            // Add item to context menu
            args.AddCustomItem(_searchMenuItem);
            args.AddCustomItem(_lodestoneMenuItem);
        }

        private void OnOpenPlayerInfo(GameObjectContextMenuItemSelectedArgs args)
        {
            // If the target name is null, return
            if (_targetFullName == null)
            {
                return;
            }

            var targetNameSplit = _targetFullName.Split(' ');
            var searchCommand = $"/search forename \"{targetNameSplit[0]}\" surname \"{targetNameSplit[1]}\"";

            // Discard return value because the call is not awaited
            _ = _plugin.ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnOpenLodestone(GameObjectContextMenuItemSelectedArgs args)
        {
            // If the target name is null, return
            if (_targetFullName == null)
            {
                return;
            }

            _plugin.LodestoneService.OpenCharacterLodestone(_targetFullName, args.ObjectWorld);
        }
    }}

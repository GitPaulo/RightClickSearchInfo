using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Gui.ContextMenu;
using RightClickSearchInfo.Util;
using RightClickSearchInfo.Windows;

namespace RightClickSearchInfo.ContextMenus
{
    public class TargetContextMenu
    {
        private readonly MenuItem searchParentMenuItem;
        private readonly MenuItem searchMenuItem;
        private readonly MenuItem ffLogsMenuItem;
        private readonly MenuItem lodestoneMenuItem;
        private readonly MenuItem ffxivCollectMenuItem;
        private readonly MenuItem lalachievmentsMenuItem;

        private string? targetFullName;
        private uint targetWorldId;

        public TargetContextMenu()
        {
            searchMenuItem = new MenuItem
            {
                Name = "in Game",
                OnClicked = OnOpenPlayerInfo,
                PrefixChar = 'S'
            };
            ffLogsMenuItem = new MenuItem
            {
                Name = "in FFLogs",
                OnClicked = OnOpenFFLogs,
                PrefixChar = 'S'
            };
            lodestoneMenuItem = new MenuItem
            {
                Name = "in Lodestone",
                OnClicked = OnOpenLodestone,
                PrefixChar = 'S'
            };
            ffxivCollectMenuItem = new MenuItem
            {
                Name = "in FFXIV Collect",
                OnClicked = OnOpenFFXIVCollect,
                PrefixChar = 'S'
            };
            lalachievmentsMenuItem = new MenuItem
            {
                Name = "in LalaAchievements",
                OnClicked = OnOpenLalaAchievements,
                PrefixChar = 'S'
            };

            // Initialize the parent menu item
            searchParentMenuItem = new MenuItem
            {
                Name = "Search",
                IsSubmenu = true,
                OnClicked = OpenSearchSubmenu, // Dynamically open submenu items
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

        private void OpenCustomSearch(CustomSearchProvider provider)
        {
            if (targetFullName == null)
                return;

            var world = WorldUtils.WorldIdToName(targetWorldId);
            var encodedName = Uri.EscapeDataString(targetFullName);
            var encodedWorld = Uri.EscapeDataString(world);

            // Split name
            var nameParts = targetFullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";
    
            var lodestoneId = Shared.LodestoneService.GetLodestoneId(targetFullName, world) ?? "unknown";

            var url = provider.UrlTemplate
                              .Replace("$1", encodedName)
                              .Replace("$2", encodedWorld)
                              .Replace("$3", Uri.EscapeDataString(lodestoneId))
                              .Replace("$first", Uri.EscapeDataString(firstName))
                              .Replace("$last", Uri.EscapeDataString(lastName));
            
            Dalamud.Utility.Util.OpenLink(url);
            Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
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

            // Add the parent "Search" menu item
            menuArgs.AddMenuItem(searchParentMenuItem);
        }

        private void OpenSearchSubmenu(IMenuItemClickedArgs args)
        {
            // Create the submenu items dynamically
            var submenuItems = new List<MenuItem>();

            if (Shared.Config.ShowSearchInfoItem)
            {
                submenuItems.Add(searchMenuItem);
            }

            if (Shared.Config.ShowLodestoneItem)
            {
                submenuItems.Add(lodestoneMenuItem);
            }

            if (Shared.Config.ShowFFXIVCollectItem)
            {
                submenuItems.Add(ffxivCollectMenuItem);
            }

            if (Shared.Config.ShowLalaAchievementsItem)
            {
                submenuItems.Add(lalachievmentsMenuItem);
            }

            // Check if FFLogs Shared is enabled
            var isFFLogsEnabled =
                Shared.PluginInterface.InstalledPlugins.Any(pluginInfo =>
                                                                pluginInfo.InternalName == "FFLogsViewer" &&
                                                                pluginInfo.IsLoaded) && Shared.Config.ShowFFLogsItem;
            if (!isFFLogsEnabled)
            {
                submenuItems.Add(ffLogsMenuItem);
            }

            // Add user-defined search providers
            foreach (var provider in Shared.Config.CustomSearchProviders)
            {
                var dynamicMenuItem = new MenuItem
                {
                    Name = provider.Label,
                    PrefixChar = 'S',
                    OnClicked = _ => OpenCustomSearch(provider)
                };
                submenuItems.Add(dynamicMenuItem);
            }

            args.OpenSubmenu("Search", submenuItems);
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

        private void OnOpenLalaAchievements(IMenuItemClickedArgs args)
        {
            if (targetFullName == null)
            {
                return;
            }

            Shared.LalaAchievementsService.OpenCharacterLalaAchievements(targetFullName, targetWorldId);
        }
    }
}

using System.IO;

using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.ContextMenu;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;

using RightClickSearchInfo.Windows;
using RightClickSearchInfo.ContextMenus;
using RightClickSearchInfo.Services;
using RightClickSearchInfo.Sound;

namespace RightClickSearchInfo
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService]
        [RequiredVersion("1.0")]
        public static ClientState ClientState { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static ChatGui ChatGui { get; private set; } = null!;

        public SearchCommandService SearchCommandService { get; set; } = null!;

        public string Name => "Right Click Search info";
        private const string MainCommand = "/rcsi";
        private const string MouseOverCommand = "/seamo";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }

        public WindowSystem WindowSystem = new("RightClickSearchInfo");
        public DalamudContextMenu ContextMenu = null!;

        private MainWindow MainWindow { get; init; }
        private TargetContextMenu TargetContextMenu { get; init; }

        private readonly TargetManager targetManager;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            TargetManager targetManager) 
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.targetManager = targetManager;

            // Resources
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
            var notifPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "notif.mp3");

            // Services
            this.SearchCommandService = new SearchCommandService(this, notifPath);

            // Windows
            MainWindow = new MainWindow(this, goatImage);
            WindowSystem.AddWindow(MainWindow);

            // Context Menu
            this.ContextMenu = new DalamudContextMenu();
            TargetContextMenu = new TargetContextMenu(this);

            // Commands
            this.CommandManager.AddHandler(MainCommand, new CommandInfo(OnMainCommand)
            {
                HelpMessage = "Usage instructions."
            });
            this.CommandManager.AddHandler(MouseOverCommand, new CommandInfo(OnMouseOverCommand)
            {
                HelpMessage = "Search info command for mouse over target."
            });

            // Hooks
            this.PluginInterface.UiBuilder.Draw += DrawUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            MainWindow.Dispose();
            TargetContextMenu.Dispose();

            this.CommandManager.RemoveHandler(MainCommand);
            this.CommandManager.RemoveHandler(MouseOverCommand);
        }

        private void OnMainCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void OnMouseOverCommand(string command, string args)
        {
            GameObject? target = targetManager.MouseOverTarget;
            if (target == null || target.ObjectKind != ObjectKind.Player) return;

            string targetFullName = target.Name.ToString();
            this.SearchCommandService.runSearch(targetFullName);
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }
    }
}


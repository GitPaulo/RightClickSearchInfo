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

using RightClickSearchInfo.Windows;
using RightClickSearchInfo.ContextMenus;

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

        public string Name => "Right Click Search info";
        private const string CommandName = "/rcsi";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }

        public WindowSystem WindowSystem = new("RightClickSearchInfo");
        public DalamudContextMenu ContextMenu = null!;

        private MainWindow MainWindow { get; init; }
        private TargetContextMenu TargetContextMenu { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            // Goat resource
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            // Windows
            MainWindow = new MainWindow(this, goatImage); 
            WindowSystem.AddWindow(MainWindow);

            // Context Menu
            this.ContextMenu = new DalamudContextMenu();
            TargetContextMenu = new TargetContextMenu(this);

            // CMD 
            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Usage instructions..."
            });

            // Hooks
            this.PluginInterface.UiBuilder.Draw += DrawUI;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            MainWindow.Dispose();
            TargetContextMenu.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }
    }
}

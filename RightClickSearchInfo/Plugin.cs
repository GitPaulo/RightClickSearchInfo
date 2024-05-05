using System.IO;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using RightClickSearchInfo.ContextMenus;
using RightClickSearchInfo.Services;
using RightClickSearchInfo.Sound;
using RightClickSearchInfo.Windows;

namespace RightClickSearchInfo
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string MainCommand = "/rcsi";
        private const string SearchOverCommand = "/seamo";
        private const string LodestoneOverCommand = "/lodmo";
        private readonly MainWindow _mainWindow;
        private readonly TargetContextMenu _targetContextMenu;
        private readonly WindowSystem _windowSystem = new("RightClickSearchInfo");
        
        public Resources PluginResources { get; }
        public static IChatGui? ChatGui { get; private set; }
        public static IClientState? ClientState { get; private set; }
        public IDataManager DataManager { get; init; }
        public ICommandManager CommandManager { get; init; }
        public ITargetManager TargetManager { get; init; }
        public IPluginLog PluginLog { get; init; }
        public ChatAutomationService ChatAutomationService { get; set; }
        public LodestoneService LodestoneService { get; set; }
        public SoundEngine SoundEngine { get; set; }
        public DalamudPluginInterface PluginInterface { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] IChatGui? chatGui,
            [RequiredVersion("1.0")] IClientState? clientState,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] ITargetManager targetManager,
            [RequiredVersion("1.0")] IDataManager dataManager,
            [RequiredVersion("1.0")] IPluginLog pluginLog
        )
        {
            ChatGui = chatGui;
            ClientState = clientState;
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            TargetManager = targetManager;
            DataManager = dataManager;
            PluginLog = pluginLog;

            // Resources
            var assemblyDirectory = pluginInterface.AssemblyLocation.Directory?.FullName!;
            var goatPath = Path.Combine(assemblyDirectory, "goat.png");
            var notifPath = Path.Combine(assemblyDirectory, "notif.mp3");
            PluginResources = new Resources(goatPath, notifPath);

            // Sound
            SoundEngine = new SoundEngine(pluginLog);
            
            // Services
            ChatAutomationService = new ChatAutomationService(this);
            LodestoneService = new LodestoneService(this);

            // Windows
            _mainWindow = new MainWindow(this);
            _windowSystem.AddWindow(_mainWindow);

            // Context Menu
            _targetContextMenu = new TargetContextMenu(this);
            
            // Commands
            CommandManager.AddHandler(MainCommand, new CommandInfo(OnMainCommand)
            {
                HelpMessage = "Usage instructions."
            });
            CommandManager.AddHandler(SearchOverCommand, new CommandInfo(OnSearchOverCommand)
            {
                HelpMessage = "Search info command for mouse over target."
            });
            CommandManager.AddHandler(LodestoneOverCommand, new CommandInfo(OnLodestoneOverCommand)
            {
                HelpMessage = "Lodestone command for mouse over target."
            });

            // Hooks
            PluginInterface.UiBuilder.Draw += DrawUi;
        }
        
        public void Dispose()
        {
            _windowSystem.RemoveAllWindows();
            _mainWindow.Dispose();
            _targetContextMenu.Dispose();

            CommandManager.RemoveHandler(MainCommand);
            CommandManager.RemoveHandler(SearchOverCommand);
            CommandManager.RemoveHandler(LodestoneOverCommand);
        }

        private void OnMainCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            _mainWindow.IsOpen = true;
        }

        private void OnSearchOverCommand(string command, string args)
        {
            var target = TargetManager.MouseOverTarget;
            if (target == null || target.ObjectKind != ObjectKind.Player) return;

            var targetFullName = target.Name.ToString();
            var targetNameSplit = targetFullName.Split(' ');
            var searchCommand = $"/search forename \"{targetNameSplit[0]}\" surname \"{targetNameSplit[1]}\"";

            #pragma warning disable CS4014 
            ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnLodestoneOverCommand(string command, string args)
        {
            var target = TargetManager.MouseOverTarget;
            if (target == null || target.ObjectKind != ObjectKind.Player) return;

            var targetFullName = target.Name.ToString();
            var worldId = ClientState?.LocalPlayer?.CurrentWorld.Id;

            if (worldId != null)
            {
                LodestoneService.OpenCharacterLodestone(targetFullName, worldId.Value);
            }
        }

        private void DrawUi()
        {
            _windowSystem.Draw();
        }

        public readonly struct Resources(string goatPath, string notificationPath)
        {
            public string NotificationPath { get; } = notificationPath;
        }
    }
}

using System.IO;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
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
        private const string FFXIVCollectOverCommand = "/fcmo";
        
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
        public IContextMenu ContextMenu {  get; init; }
        public ChatAutomationService ChatAutomationService { get; set; }
        public LodestoneService LodestoneService { get; set; }
        public SearchInfoCommandService SearchInfoCommandService { get; set; }
        public FFXIVCollectService FFXIVCollectService { get; set; }
        public FFLogsService FFLogsService { get; set; }
        public SoundEngine SoundEngine { get; set; }
        public IDalamudPluginInterface PluginInterface { get; init; }

        public Plugin(
            IDalamudPluginInterface pluginInterface,
            IChatGui? chatGui,
            IClientState? clientState,
            ICommandManager commandManager,
            ITargetManager targetManager,
            IDataManager dataManager,
            IPluginLog pluginLog,
            IContextMenu contextMenu
        )
        {
            ChatGui = chatGui;
            ClientState = clientState;
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            TargetManager = targetManager;
            DataManager = dataManager;
            PluginLog = pluginLog;
            ContextMenu = contextMenu;

            // Resources
            var assemblyDirectory = pluginInterface.AssemblyLocation.Directory?.FullName!;
            var notifPath = Path.Combine(assemblyDirectory, "notif.mp3");
            PluginResources = new Resources(notifPath);

            // Sound
            SoundEngine = new SoundEngine(pluginLog);
            
            // Services
            ChatAutomationService = new ChatAutomationService(this);
            LodestoneService = new LodestoneService(this);
            SearchInfoCommandService = new SearchInfoCommandService(this);
            FFXIVCollectService = new FFXIVCollectService(this);
            FFLogsService = new FFLogsService(this);

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
            CommandManager.AddHandler(FFXIVCollectOverCommand, new CommandInfo(OnFFXIVCollectOverCommand)
            {
                HelpMessage = "FFXIV Collect command for mouse over target."
            });

            // Hooks
            PluginInterface.UiBuilder.Draw += DrawUi;
            _targetContextMenu.Enable();
        }
        
        public void Dispose()
        {
            _windowSystem.RemoveAllWindows();
            _mainWindow.Dispose();
            _targetContextMenu.Disable();

            CommandManager.RemoveHandler(MainCommand);
            CommandManager.RemoveHandler(SearchOverCommand);
            CommandManager.RemoveHandler(LodestoneOverCommand);
            CommandManager.RemoveHandler(FFXIVCollectOverCommand);
            
            PluginInterface.UiBuilder.Draw -= DrawUi;
        }

        private void OnMainCommand(string command, string args)
        {
            _mainWindow.IsOpen = true;
        }

        private void OnSearchOverCommand(string command, string args)
        {
            var target = TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            string searchCommand = SearchInfoCommandService.CreateCommandString(target);
            _ = ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnLodestoneOverCommand(string command, string args)
        {
            var target = TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            var targetFullName = target.Name.ToString();
            var worldId = target.HomeWorld.Id;
            LodestoneService.OpenCharacterLodestone(targetFullName, worldId);
        }

        private void OnFFXIVCollectOverCommand(string command, string args)
        {
            
            var target = TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            var targetFullName = target.Name.ToString();
            var worldId = target.HomeWorld.Id;
            FFXIVCollectService.OpenCharacterFFXIVCollect(targetFullName, worldId);
        }

        private void DrawUi()
        {
            _windowSystem.Draw();
        }

        public readonly struct Resources(string notificationPath)
        {
            public string NotificationPath { get; } = notificationPath;
        }
    }
}

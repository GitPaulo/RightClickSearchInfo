using System.IO;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using RightClickSearchInfo.ContextMenus;
using RightClickSearchInfo.Services;
using RightClickSearchInfo.Sound;
using RightClickSearchInfo.Windows;

namespace RightClickSearchInfo
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string Name = "RightClickSearchInfo";
        private const string MainCommand = "/rcsi";
        private const string SearchOverCommand = "/seamo";
        private const string LodestoneOverCommand = "/lodmo";
        private const string FFXIVCollectOverCommand = "/fcmo";
        private const string FFLogsOverCommand = "/fflmo";

        private TargetContextMenu targetContextMenu;
        private readonly WindowSystem windowSystem = new(Name);

        public Plugin(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Shared>();

            Shared.Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            InitWindows();
            InitCommands();
            InitServices();
            InitResources();
            InitSound();
            InitContextMenu();
            InitHooks();

            Shared.Log.Information($"Loaded {Shared.PluginInterface.Manifest.Name}");
        }

        private void InitWindows()
        {
            Shared.ConfigWindow = new ConfigWindow();
            Shared.MainWindow = new MainWindow();

            windowSystem.AddWindow(Shared.MainWindow);
            windowSystem.AddWindow(Shared.ConfigWindow);
        }

        private void InitCommands()
        {
            // Commands
            // I know these are kind of dumb but its my first Dalamud plugin IDC
            Shared.CommandManager.AddHandler(MainCommand, new CommandInfo(OnMainCommand)
            {
                HelpMessage = "Usage instructions."
            });
            Shared.CommandManager.AddHandler(SearchOverCommand, new CommandInfo(OnSearchOverCommand)
            {
                HelpMessage = "Search info command for mouse over target."
            });
            Shared.CommandManager.AddHandler(LodestoneOverCommand, new CommandInfo(OnLodestoneOverCommand)
            {
                HelpMessage = "Lodestone command for mouse over target."
            });
            Shared.CommandManager.AddHandler(FFXIVCollectOverCommand, new CommandInfo(OnFFXIVCollectOverCommand)
            {
                HelpMessage = "FFXIV Collect command for mouse over target."
            });
            Shared.CommandManager.AddHandler(FFLogsOverCommand, new CommandInfo(OnFFLogsOverCommand)
            {
                HelpMessage = "FFLogs command for mouse over target."
            });
        }

        private void InitServices()
        {
            Shared.ChatAutomationService = new ChatAutomationService();
            Shared.LodestoneService = new LodestoneService();
            Shared.SearchInfoCommandService = new SearchInfoCommandService();
            Shared.FFXIVCollectService = new FFXIVCollectService();
            Shared.FFLogsService = new FFLogsService();
        }

        private void InitResources()
        {
            var assemblyDirectory = Shared.PluginInterface.AssemblyLocation.Directory?.FullName!;
            var notifPath = Path.Combine(assemblyDirectory, "notif.mp3");

            Shared.SoundNotificationPath = notifPath;
        }

        private void InitSound()
        {
            Shared.SoundEngine = new SoundEngine();
        }

        private void InitContextMenu()
        {
            targetContextMenu = new TargetContextMenu();
            targetContextMenu.Enable();
        }

        private void InitHooks()
        {
            Shared.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
            Shared.PluginInterface.UiBuilder.Draw += DrawUi;
        }

        public void Dispose()
        {
            windowSystem.RemoveAllWindows();

            Shared.CommandManager.RemoveHandler(MainCommand);
            Shared.CommandManager.RemoveHandler(SearchOverCommand);
            Shared.CommandManager.RemoveHandler(LodestoneOverCommand);
            Shared.CommandManager.RemoveHandler(FFXIVCollectOverCommand);
            Shared.CommandManager.RemoveHandler(FFLogsOverCommand);

            Shared.PluginInterface.UiBuilder.Draw -= DrawUi;
            targetContextMenu.Disable();
        }

        public void ToggleConfigUI()
        {
            Shared.ConfigWindow.Toggle();
        }

        private void OnMainCommand(string command, string args)
        {
            Shared.MainWindow.Toggle();
        }

        private void OnSearchOverCommand(string command, string args)
        {
            var target = Shared.TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            string searchCommand = Shared.SearchInfoCommandService.CreateCommandString(target);
            _ = Shared.ChatAutomationService.SendMessage(searchCommand);
        }

        private void OnLodestoneOverCommand(string command, string args)
        {
            var target = Shared.TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            var targetFullName = target.Name.ToString();
            var worldId = target.HomeWorld.RowId;
            Shared.LodestoneService.OpenCharacterLodestone(targetFullName, worldId);
        }

        private void OnFFXIVCollectOverCommand(string command, string args)
        {
            var target = Shared.TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            var targetFullName = target.Name.ToString();
            var worldId = target.HomeWorld.RowId;
            Shared.FFXIVCollectService.OpenCharacterFFXIVCollect(targetFullName, worldId);
        }

        private void OnFFLogsOverCommand(string command, string args)
        {
            var target = Shared.TargetManager.MouseOverTarget as IPlayerCharacter;
            if (target == null || target.ObjectKind != ObjectKind.Player)
            {
                return;
            }

            var targetFullName = target.Name.ToString();
            Shared.FFLogsService.OpenCharacterFFLogs(targetFullName);
        }

        private void DrawUi()
        {
            windowSystem.Draw();
        }
    }
}

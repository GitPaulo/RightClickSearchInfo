using System.IO;
using Dalamud.ContextMenu;
using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using RightClickSearchInfo.ContextMenus;
using RightClickSearchInfo.Services;
using RightClickSearchInfo.Windows;

namespace RightClickSearchInfo;

public sealed class Plugin : IDalamudPlugin
{
    private const string MainCommand = "/rcsi";
    private const string SearchOverCommand = "/seamo";
    private const string LodestoneOverCommand = "/lodmo";

    public readonly Resources PluginResources;
    private readonly TargetManager TargetManager;
    public DalamudContextMenu ContextMenu = null!;


    public WindowSystem WindowSystem = new("RightClickSearchInfo");

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] TargetManager targetManager,
        [RequiredVersion("1.0")] DataManager dataManager
    )
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        TargetManager = targetManager;
        DataManager = dataManager;

        // Resources
        var goatPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
        var notifPath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "notif.mp3");
        PluginResources = new Resources(goatPath, notifPath);

        // Services
        ChatAutomationService = new ChatAutomationService(this);
        LodestoneService = new LodestoneService(this);

        // Windows
        MainWindow = new MainWindow(this);
        WindowSystem.AddWindow(MainWindow);

        // Context Menu
        ContextMenu = new DalamudContextMenu();
        TargetContextMenu = new TargetContextMenu(this);

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
        PluginInterface.UiBuilder.Draw += DrawUI;
    }

    [PluginService]
    [RequiredVersion("1.0")]
    public static ClientState ClientState { get; private set; } = null!;

    [PluginService]
    [RequiredVersion("1.0")]
    public static ChatGui ChatGui { get; private set; } = null!;

    public ChatAutomationService ChatAutomationService { get; set; } = null!;
    public LodestoneService LodestoneService { get; set; } = null!;

    public DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    public DataManager DataManager { get; init; }

    private MainWindow MainWindow { get; init; }
    private TargetContextMenu TargetContextMenu { get; init; }

    public string Name => "Right Click Search info";

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();
        TargetContextMenu.Dispose();

        CommandManager.RemoveHandler(MainCommand);
        CommandManager.RemoveHandler(SearchOverCommand);
        CommandManager.RemoveHandler(LodestoneOverCommand);
    }

    private void OnMainCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        MainWindow.IsOpen = true;
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

    private void DrawUI()
    {
        WindowSystem.Draw();
    }


    public struct Resources
    {
        public string GoatPath { get; }
        public string NotificationPath { get; }

        public Resources(string goatPath, string notificationPath)
        {
            GoatPath = goatPath;
            NotificationPath = notificationPath;
        }
    }
}

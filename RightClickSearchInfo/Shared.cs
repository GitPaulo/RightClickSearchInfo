using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using RightClickSearchInfo.Services;
using RightClickSearchInfo.Sound;
using RightClickSearchInfo.Windows;

namespace RightClickSearchInfo;

internal class Shared
{
    public static Configuration Config { get; set; } = null!;
    public static ConfigWindow ConfigWindow { get; set; } = null!;
    public static MainWindow MainWindow { get; set; }
    public static ChatAutomationService ChatAutomationService { get; set; } = null!;
    public static LodestoneService LodestoneService { get; set; } = null!;
    public static SearchInfoCommandService SearchInfoCommandService { get; set; } = null!;
    public static FFXIVCollectService FFXIVCollectService { get; set; } = null!;
    public static FFLogsService FFLogsService { get; set; } = null!;
    public static SoundEngine SoundEngine { get; set; } = null!;
    public static string SoundNotificationPath { get; set; } = null!;
    
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui Chat { get; private set; } = null!;
    [PluginService] internal static IContextMenu ContextMenu { get; private set; } = null!;
    [PluginService] internal static ITargetManager TargetManager { get; private set; } = null!;
}

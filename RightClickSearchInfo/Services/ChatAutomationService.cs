using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using RightClickSearchInfo.Sound;

using System.Threading.Tasks;

namespace RightClickSearchInfo.Services;

public class ChatAutomationService
{
    private readonly Plugin plugin;
    private readonly InputSimulator inputSimulator;

    public readonly int CHAT_ACTION_BUFFER_MS = 750;
    
    public ChatAutomationService(Plugin Plugin)
    {
        this.plugin = Plugin;
        inputSimulator = new InputSimulator();
    }

    public async Task SendMessage(string commandStr)
    {
        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        inputSimulator.Keyboard.TextEntry(commandStr);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        // Confirmation
        Plugin.ChatGui.Print("[AUTOMATED]: " + commandStr);
        SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }
}

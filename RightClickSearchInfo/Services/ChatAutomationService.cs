using WindowsInput;
using WindowsInput.Native;

using System.Threading.Tasks;

namespace RightClickSearchInfo.Services;

public class ChatAutomationService(Plugin plugin)
{
    private readonly InputSimulator _inputSimulator = new();

    private const int CHAT_ACTION_BUFFER_MS = 750;

    public async Task SendMessage(string commandStr)
    {
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        _inputSimulator.Keyboard.TextEntry(commandStr);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        // Confirmation
        Plugin.ChatGui.Print("[AUTOMATED]: " + commandStr);
        plugin.SoundEngine.PlaySound(plugin.PluginResources.NotificationPath);
    }
}

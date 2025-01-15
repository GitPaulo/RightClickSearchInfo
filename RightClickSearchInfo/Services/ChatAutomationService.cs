using WindowsInput;
using WindowsInput.Native;

using System.Threading.Tasks;

namespace RightClickSearchInfo.Services;

public class ChatAutomationService()
{
    private readonly InputSimulator inputSimulator = new();

    private const int CHAT_ACTION_BUFFER_MS = 750;

    public async Task SendMessage(string commandStr)
    {
        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        inputSimulator.Keyboard.TextEntry(commandStr);

        await Task.Delay(CHAT_ACTION_BUFFER_MS);
        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        // Confirmation
        Shared.Chat.Print("[AUTOMATED]: " + commandStr);
        Shared.SoundEngine.PlaySound(Shared.SoundNotificationPath);
    }
}

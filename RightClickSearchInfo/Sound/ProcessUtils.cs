using System;
using System.Runtime.InteropServices;

namespace RightClickSearchInfo.Sound;

public static class ProcessUtils
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public static uint GetForegroundProcessId()
    {
        var hWnd = GetForegroundWindow(); // Get foreground window handle
        GetWindowThreadProcessId(hWnd, out var processId);
        return processId;
    }
}

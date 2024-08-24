using Dalamud.Utility;
using System;

namespace RightClickSearchInfo.Services;

public class FFLogsService(Plugin plugin)
{
    public void OpenCharacterFFLogs(string fullName)
    {
        var encodedFullName = Uri.EscapeDataString(fullName);
        Util.OpenLink($"https://www.fflogs.com/search/?term={encodedFullName}");
    }
}

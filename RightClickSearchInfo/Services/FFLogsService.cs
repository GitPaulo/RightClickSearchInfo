using System;

namespace RightClickSearchInfo.Services;

public class FFLogsService()
{
    public void OpenCharacterFFLogs(string fullName)
    {
        var encodedFullName = Uri.EscapeDataString(fullName);
        Dalamud.Utility.Util.OpenLink($"https://www.fflogs.com/search/?term={encodedFullName}");
    }
}

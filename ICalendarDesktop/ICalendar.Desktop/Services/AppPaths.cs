using System;
using System.IO;

namespace ICalendar.Desktop.Services;

public static class AppPaths
{
    private const string AppFolderName = "ICalendarDesktop";

    public static string CredentialsPath => Path.Combine(AppContext.BaseDirectory, "credentials.json");

    public static string TokenPath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppFolderName,
                "token");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}

namespace SnapFocus.Core;

/// <summary>
/// Ultra-early logger for "does the process even start?".
/// Writes to %TEMP%\SnapFocus_boot.log
/// </summary>
public static class BootLog
{
    private static readonly string PathFile =
        System.IO.Path.Combine(System.IO.Path.GetTempPath(), "SnapFocus_boot.log");

    public static void Write(string line)
    {
        try
        {
            System.IO.File.AppendAllText(PathFile, $"{System.DateTime.Now:O} {line}{System.Environment.NewLine}");
        }
        catch { /* ignore */ }
    }
}

using SnapFocus.Core.Paths;

namespace SnapFocus.Core.Logging;

/// <summary>
/// File logger that works unpackaged and MSIX-packaged.
/// In MSIX, LocalApplicationData is redirected to:
/// %LOCALAPPDATA%\Packages\<PFN>\LocalCache\Local
/// </summary>
public sealed class FileLogger : ILogger
{
    private readonly object _gate = new();

    public string LogDirectory { get; }
    public string SessionLogFile { get; }
    public string LatestLogFile { get; }

    public FileLogger(string appName = "SnapFocus")
    {
        //var baseDir = Environment.GetFolderPath(
        //    Environment.SpecialFolder.LocalApplicationData);

        //LogDirectory = Path.Combine(baseDir, appName, "logs");
        LogDirectory = AppPaths.GetLogDirectory(appName);
        DirectoryInfo LogFilePathInfo = Directory.CreateDirectory(LogDirectory);

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        SessionLogFile = Path.Combine(LogDirectory, $"{appName}_{stamp}.log");
        LatestLogFile = Path.Combine(LogDirectory, $"{appName}.log");

        Info($"Logger initialized. logDir={LogDirectory} session={SessionLogFile} latest={LatestLogFile}");

    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);

    public void Error(string message, Exception? ex = null)
        => Write("ERROR", ex == null ? message : $"{message} | {ex.GetType().Name}: {ex.Message}\n{ex}");

    private void Write(string level, string message)
    {
        lock (_gate)
        {
            WriteInternal(level, message);
        }
    }

    private void WriteInternal(string level, string message)
    {
        var line = $"[{DateTime.Now:O}] {level} {message}{Environment.NewLine}";
        File.AppendAllText(SessionLogFile, line);
        File.AppendAllText(LatestLogFile, line);
    }
}

using System.Text;

namespace SnapFocus.Core.Logging;

/// <summary>
/// File logger that works unpackaged and MSIX-packaged.
/// In MSIX, LocalApplicationData is redirected to:
/// %LOCALAPPDATA%\Packages\<PFN>\LocalCache\Local
/// </summary>
public sealed class FileLogger : ILogger
{
    private readonly object _gate = new();
    private readonly string _logFile;

    public string LogFilePath => _logFile;
    public string LogFolderPath; // => Path.GetDirectoryName(_logFile)!;

    public FileLogger(string appName = "SnapFocus")
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(baseDir, appName, "logs");
        DirectoryInfo LogFilePathInfo = Directory.CreateDirectory(dir);

        LogFolderPath = dir;

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _logFile = Path.Combine(dir, $"{appName}_{stamp}.log");

        Info($"Logger initialized. baseDir={baseDir} logDir={dir}");
    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);

    public void Error(string message, Exception? ex = null)
        => Write("ERROR", ex == null ? message : $"{message} | {ex.GetType().Name}: {ex.Message}\n{ex}");

    private void Write(string level, string message)
    {
        var line = $"[{DateTime.Now:O}] {level} {message}{Environment.NewLine}";
        lock (_gate)
        {
            File.AppendAllText(_logFile, line, Encoding.UTF8);
        }
    }
}

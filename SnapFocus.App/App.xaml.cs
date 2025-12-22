using System.Windows;
using System.Threading;
using SnapFocus.Core;
using SnapFocus.Core.Logging;
using SnapFocus.Core.Interfaces;
using SnapFocus.Input;
using SnapFocus.Layouts;
using SnapFocus.Observer;
using System.IO;
using Application = System.Windows.Application;


namespace SnapFocus.App;

public partial class App : System.Windows.Application
{
    private FileLogger? _logger;
    private SingleInstance? _singleInstance;
    private TrayService? _tray;
    private IWindowObserver? _observer;
    private IHotkeyService? _hotkeys;
    private ILayoutProvider? _layouts;

    protected override void OnStartup(StartupEventArgs e)
    {
        BootLog.Write("=== BOOT INIT ===");
        BootLog.Write($"=== BOOT Instance PID={Environment.ProcessId} ===");

        _singleInstance = new SingleInstance(@"Global\whami.SnapFocus.SingleInstance");
        if (!_singleInstance.IsPrimary)
        {
            BootLog.Write($"=== BOOT EXIT \"secondary instance blocked\" PID={Environment.ProcessId} ===");
            Shutdown();
            return;
        }


        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            _logger = new FileLogger("SnapFocus");
            _logger.Info("App.OnStartup entered");
            _logger.Info($"ProcessId={Environment.ProcessId}");
            _logger.Info($"Exe={Environment.ProcessPath}");
            _logger.Info($"CurrentDir={Environment.CurrentDirectory}");
            _logger.Info($"Temp={Path.GetTempPath()}");
            _logger.Info($"LocalAppData={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}");
            _logger.Info($"LogFile={_logger.SessionLogFile}");

            // Initialize modules (no-op but fully wired & logged)
            _observer = new WindowObserver(_logger);
            _layouts  = new EmptyLayoutProvider(_logger);
            _hotkeys  = new HotkeyService(_logger);

            _logger.Info("Loading modules: Observer, Layouts, Hotkeys");
            _observer.Start();
            _layouts.Refresh();
            _hotkeys.Start();

            _tray = new TrayService(_logger);
            _tray.ShowStartupBalloon(); // optional "mongo wow"

            _logger.Info("Application running (no-op mode)");
        }
        catch (Exception ex)
        {
            try { _logger?.Error("Fatal error during startup", ex); } catch { }
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            _logger?.Info("App.OnExit entered");
            _hotkeys?.Stop();
            _observer?.Stop();
            _tray?.Dispose();
            _singleInstance?.Dispose();
            _logger?.Info("Shutdown complete");
        }
        catch { /* ignore */ }

        base.OnExit(e);
    }
}

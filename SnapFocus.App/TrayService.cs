using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using SnapFocus.Core.Logging;
using Forms = System.Windows.Forms;

namespace SnapFocus.App;

/// <summary>
/// Tray UX for Mongos:
/// - Left-click opens menu (not only right-click)
/// - Open Logs (Notepad)
/// - Open Logs Folder (Explorer)
/// - Open Startup Apps settings
/// - Exit (clean shutdown)
/// </summary>
public sealed class TrayService : IDisposable
{
    private ILogger _logger;
    private readonly NotifyIcon _icon;
    private readonly ContextMenuStrip _menu;

    public TrayService(FileLogger logger)
    {
        _logger = logger;

        _menu = BuildMenu(logger);

        _icon = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "SnapFocus",
            ContextMenuStrip = _menu
        };

        // Best-effort set to our placeholder icon (works unpackaged; in MSIX may fall back)
        try
        {
            var icoPath = Path.Combine(AppContext.BaseDirectory, "snapfocus-placeholder.ico");
            if (File.Exists(icoPath))
                _icon.Icon = new System.Drawing.Icon(icoPath);
        }
        catch { /* ignore */ }

        _icon.MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                // Show context menu on left click (mongo wow)
                _menu.Show(Cursor.Position);
            }
        };

        _logger.Info("TrayService initialized (NotifyIcon visible)");
    }

    private ContextMenuStrip BuildMenu(FileLogger logger)
    {
        var menu = new ContextMenuStrip();

        var openLogs = new ToolStripMenuItem("Open Logs (Notepad)", null, (_, __) =>
        {
            try
            {
                _logger.Info("Tray: Open Logs (Notepad)");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{logger.LogFilePath}\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex) { _logger.Error("Open Logs failed", ex); }
        });

        var openFolder = new ToolStripMenuItem("Open Logs Folder", null, (_, __) =>
        {
            try
            {
                _logger.Info("Tray: Open Logs Folder");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{logger.LogFolderPath}\"",
                    UseShellExecute = true
                });
            }
            catch (Exception ex) { _logger.Error("Open Logs Folder failed", ex); }
        });

        var startup = new ToolStripMenuItem("Open Windows Startup Apps", null, (_, __) =>
        {
            try
            {
                _logger.Info("Tray: Open Startup Apps settings");
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:startupapps",
                    UseShellExecute = true
                });
            }
            catch (Exception ex) { _logger.Error("Open Startup Apps failed", ex); }
        });

        var exit = new ToolStripMenuItem("Exit", null, (_, __) =>
        {
            _logger.Info("Tray: Exit clicked");
            System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Shutdown());
        });

        menu.Items.Add(openLogs);
        menu.Items.Add(openFolder);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(startup);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exit);

        return menu;
    }

    public void ShowStartupBalloon()
    {
        try
        {
            _icon.ShowBalloonTip(2000, "SnapFocus", "Running (no-op) • Right/Left click for menu", ToolTipIcon.Info);
        }
        catch { /* ignore */ }
    }

    public void Dispose()
    {
        try
        {
            _logger.Info("TrayService disposing");
            _icon.Visible = false;
            _icon.Dispose();
            _menu.Dispose();
        }
        catch { /* ignore */ }
    }
}

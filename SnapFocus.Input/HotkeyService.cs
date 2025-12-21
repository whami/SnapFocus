using SnapFocus.Core.Interfaces;
using SnapFocus.Core.Logging;

namespace SnapFocus.Input;

public sealed class HotkeyService(ILogger logger) : IHotkeyService
{
    public void Start() => logger.Info("HotkeyService.Start (no-op)");
    public void Stop() => logger.Info("HotkeyService.Stop (no-op)");
}

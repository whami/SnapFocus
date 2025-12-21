using SnapFocus.Core.Interfaces;
using SnapFocus.Core.Logging;

namespace SnapFocus.Observer;

public sealed class WindowObserver(ILogger logger) : IWindowObserver
{
    public void Start() => logger.Info("WindowObserver.Start (no-op, no windows tracked yet)");
    public void Stop() => logger.Info("WindowObserver.Stop (no-op)");
}

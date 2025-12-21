using SnapFocus.Core.Interfaces;
using SnapFocus.Core.Logging;

namespace SnapFocus.Layouts;

public sealed class EmptyLayoutProvider(ILogger logger) : ILayoutProvider
{
    public void Refresh() => logger.Info("EmptyLayoutProvider.Refresh (no-op, no groups)");
}

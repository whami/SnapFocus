using System;
using System.Threading;

namespace SnapFocus.App;

internal sealed class SingleInstance : IDisposable
{
    private readonly Mutex _mutex;
    public bool IsPrimary { get; }

    public SingleInstance(string mutexName)
    {
        if (string.IsNullOrWhiteSpace(mutexName))
            throw new ArgumentException("Mutex name must not be empty.", nameof(mutexName));

        bool createdNew;
        _mutex = new Mutex(initiallyOwned: true, name: mutexName, createdNew: out createdNew);
        IsPrimary = createdNew;
    }

    public void Dispose()
    {
        try
        {
            if (IsPrimary)
                _mutex.ReleaseMutex();
        }
        catch
        {
            // ignore
        }
        finally
        {
            _mutex.Dispose();
        }
    }
}

using Serilog;

namespace YATC.Core;

public interface IYatcLogger : IDisposable
{
    void LogError(object values);
    void LogError(string message);
    void LogWarning(object values);
    void LogWarning(string message);
    void LogInfo(object values);
    void LogInfo(string message);
    void LogDebug(object values);
    void LogDebug(string message);
    void FlushAndClose();
}

public class YatcLogger : IYatcLogger
{
    public void LogError(object values)
    {
        Log.Logger.Error("{@values}", values);
    }

    public void LogError(string message)
    {
        Log.Logger.Error(message);
    }

    public void LogWarning(object values)
    {
        Log.Logger.Warning("{@values}", values);
    }

    public void LogWarning(string message)
    {
        Log.Logger.Warning(message);
    }

    public void LogInfo(object values)
    {
        Log.Logger.Information("{@values}", values);
    }

    public void LogInfo(string message)
    {
        Log.Logger.Information(message);
    }

    public void LogDebug(object values)
    {
        Log.Logger.Debug("{@values}", values);
    }

    public void LogDebug(string message)
    {
        Log.Logger.Debug(message);
    }

    public void FlushAndClose()
    {
        Log.CloseAndFlush();
    }

    public void Dispose()
    {
        FlushAndClose();
    }
}
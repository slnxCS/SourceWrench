using System;

namespace SourceWrench.Core.Services;

public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        Console.WriteLine($"INFO: {message}");
    }

    public void LogError(string message)
    {
        Console.Error.WriteLine($"ERROR: {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"WARNING: {message}");
    }
}
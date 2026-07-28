using System;

namespace SourceWrench.Core.Services;

public class ConsoleProgressReporter : IProgressReporter
{
    public void Report(int percentage, string message)
    {
        Console.WriteLine($"PROGRESS: {percentage}% - {message}");
    }

    public void Report(string message)
    {
        Console.WriteLine($"PROGRESS: {message}");
    }
}
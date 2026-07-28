namespace SourceWrench.Core.Services;

public interface ILogger
{
    void LogInfo(string message);
    void LogError(string message);
    void LogWarning(string message);
}

public interface IProgressReporter
{
    void Report(int percentage, string message);
    void Report(string message);
}
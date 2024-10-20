namespace FinBeatTech.DataProcessor.Util;

public class DatabaseLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger();
    }

    public void Dispose()
    {
    }
}
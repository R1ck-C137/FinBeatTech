using FinBeatTech.DataProcessor.Util;
using Npgsql;

namespace FinBeatTech.DataProcessor;

public class DatabaseLogger : ILogger
{
    private const string ConnectionString = "Host=localhost;Username=postgres;Password=pas;Database=FinBeat";
    private const string LogsTableSchema = "log_level text, message text, created_at timestamp without time zone";
    private const string TableName = "logs";

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var logEntry = formatter(state, exception);
        using var cmd = new NpgsqlCommand("INSERT INTO logs (log_level, message, created_at) VALUES (@LogLevel, @Message, @CreatedAt)");
        cmd.Parameters.Add(new NpgsqlParameter($"@LogLevel", logLevel.ToString()));
        cmd.Parameters.Add(new NpgsqlParameter($"@Message", logEntry));
        cmd.Parameters.Add(new NpgsqlParameter($"@CreatedAt", DateTime.UtcNow));
        PGSqlUtil.ExecuteNonQuery(cmd, ConnectionString);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => null;
}
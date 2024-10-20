using FinBeatTech.DataProcessor.Interface;
using FinBeatTech.DataProcessor.Util;
using FinBeatTech.Models;
using Npgsql;

namespace FinBeatTech.DataProcessor;

public class ValueDp : IValueDp
{
    private readonly ILogger<ValueDp> _logger;
    private readonly string? _connectionString;
    private const string ValueTableSchema = "\"Id\" integer NOT NULL, \"Code\" integer NOT NULL, \"Value\" text NOT NULL, CONSTRAINT \"ValuesTable_pkey\" PRIMARY KEY (\"Id\")";
    private const string TableName = "ValuesTable";
    
    public ValueDp(ILogger<ValueDp> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration?.GetSection("ConnectionStrings")?.GetValue<string>("ValueTable");
        if (_connectionString == null)
        {
            _logger.LogError("Config connectionString is null!");
            _connectionString = "Host=localhost;Username=postgres;Password=pas;Database=FinBeat";
        }
    }
    
    public void SaveNewValues(Dictionary<int, CodeValue> listValues)
    {
        PGSqlUtil.AutoCreateTable("ValuesTable", ValueTableSchema, _connectionString!);
        
        var insertNewValues = new NpgsqlCommand($"DELETE FROM \"{TableName}\"; " +
                                                $"INSERT INTO \"{TableName}\" ({CodeValue.GetFields()}) VALUES {CodeValue.GetValuesName(listValues.Count)}");
        
        FillParameters(listValues, insertNewValues); //Я не стал ещё деление данных на блоки делать (чтобы данные отсылались блоками по 50 строк), пишу всё пришедшее одним блоком. Я уж надеюсь через тестовое задание никто сотни записей гонять не будет.
        try
        {
            PGSqlUtil.ExecuteNonQuery(insertNewValues, _connectionString!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public Dictionary<int, CodeValue> GetValues()
    {
        var getAllValues = new NpgsqlCommand($"select {CodeValue.GetFields()} from \"{TableName}\"");
        var result = new Dictionary<int, CodeValue>();
        try
        {
            PGSqlUtil.ExecuteReader(getAllValues, rdr =>
            {
                var value = new CodeValue(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetString(2));
                result.Add(rdr.GetInt32(0), value);
            }, _connectionString!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

        return result;
    }
    
    private static void FillParameters(Dictionary<int, CodeValue> listValues, NpgsqlCommand insertNewValues)
    {
        for (var i = 0; i < listValues.Count; i++)
        {
            insertNewValues.Parameters.Add(new NpgsqlParameter($"@{i + nameof(CodeValue.Id)}", listValues[i].Id));
            insertNewValues.Parameters.Add(new NpgsqlParameter($"@{i + nameof(CodeValue.Code)}", listValues[i].Code));
            insertNewValues.Parameters.Add(new NpgsqlParameter($"@{i + nameof(CodeValue.Value)}", listValues[i].Value));
        }
    }
}
using System.Text;

namespace FinBeatTech.Models;

public class CodeValue
{
    public int Id { get; private set; }
    public int Code { get; private set; }
    public string Value { get; private set; }

    public CodeValue(int id, int code, string value)
    {
        Id = id;
        Code = code;
        Value = value;
    }

    public static string GetFields()
    {
        return @$"""{nameof(Id)}"", ""{nameof(Code)}"", ""{nameof(Value)}""";
    }

    public static string GetValuesName(int count)
    {
        var result = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            result.Append($"(@{i + nameof(Id)}, @{i + nameof(Code)}, @{i + nameof(Value)}),");
        }
        
        return result.ToString().Remove(result.Length - 1);
    }
}
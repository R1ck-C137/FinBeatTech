using FinBeatTech.BusinessLogic;
using FinBeatTech.DataProcessor.Interface;
using FinBeatTech.Models;
using Newtonsoft.Json.Linq;

namespace FinBeatTech.Service;

public class MainService : IMainLogic
{
    private readonly ILogger<MainService> _logger;
    private readonly IValueDp _valueDp;
    private Dictionary<int, CodeValue> _cacheValues = new Dictionary<int, CodeValue>();

    public MainService(ILogger<MainService> logger, IValueDp valueDp)
    {
        _logger = logger;
        _valueDp = valueDp;
    }
    
    public void SetNewValue(List<Dictionary<string, string>> values)
    {
        var listValues = SortByCode(values);
        
        UpdateCache(listValues);

        _valueDp.SaveNewValues(_cacheValues);
    }

    public JArray GetValues(int page = 1, int countPageLim = 20, ValueFilters? filters = null)
    {
        if (_cacheValues.Count == 0)
            _cacheValues = _valueDp.GetValues();
        
        var result = Filtering(filters);
        result = result.Skip((page - 1) * countPageLim).Take(countPageLim).ToDictionary();
        return ConvertValueToJObject(result);
    }

    public int GetTotalCount()
    {
        return _cacheValues.Count;
    }

    private JArray ConvertValueToJObject(Dictionary<int, CodeValue> result)
    {
        var resultJArray = new JArray();
        foreach (var record in result)
        {
            resultJArray.Add(new JObject()
            {
                {"Id", record.Value.Id},
                {"Code", record.Value.Code},
                {"Value", record.Value.Value},
            });
        }

        return resultJArray;
    }

    private Dictionary<int, CodeValue> Filtering(ValueFilters? filters)
    {
        var result = _cacheValues.ToDictionary();
        if (filters == null)
            return result;
        
        if (filters.CodeFrom != null)
        {
            result = result.Where(r => r.Value.Code >= filters.CodeFrom).ToDictionary();
        }
        
        if (filters.CodeTo != null)
        {
            result = result.Where(r => r.Value.Code <= filters.CodeTo).ToDictionary();
        } 
        
        if (filters.OrderById != null)
        {
            if(filters.OrderById == OrderBy.asc)
                result = result.OrderBy(r => r.Value.Id).ToDictionary();
            if(filters.OrderById == OrderBy.desc)
                result = result.OrderByDescending(r => r.Value.Id).ToDictionary();
        }

        return result;
    }

    private void UpdateCache(List<(int code, string value)> listValues)
    {
        _cacheValues.Clear();
        for (int i = 0; i < listValues.Count; i++)
            _cacheValues.Add(i, new CodeValue(i, listValues[i].code, listValues[i].value));
        _logger.LogInformation("_cacheValues update");
    }
    
    private List<(int code, string value)> SortByCode(List<Dictionary<string, string>> values)
    {
        var listValues = new List<(int code, string value)>();
        foreach (var value in values)
        {
            var code = int.Parse(value.Keys.First());
            listValues.Add((code, value.Values.First()));
        }
        return listValues.OrderBy(v => v.Item1).ToList();
    }
}
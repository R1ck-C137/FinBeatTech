using FinBeatTech.Models;
using Newtonsoft.Json.Linq;

namespace FinBeatTech.BusinessLogic;

public interface IMainLogic
{
    public void SetNewValue(List<Dictionary<string, string>> values);
    public JArray GetValues(int page = 1, int countPageLim = 20, ValueFilters? filters = null);
    public int GetTotalCount();
}
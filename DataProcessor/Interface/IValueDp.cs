using FinBeatTech.Models;

namespace FinBeatTech.DataProcessor.Interface;

public interface IValueDp
{
    public void SaveNewValues(Dictionary<int, CodeValue> listValues);
    public Dictionary<int, CodeValue> GetValues();
}
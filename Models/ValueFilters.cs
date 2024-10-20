namespace FinBeatTech.Models;

public class ValueFilters
{
    public int? CodeFrom { get; set; }
    public int? CodeTo { get; set; }
    public OrderBy? OrderById { get; set; }

    public ValueFilters(int? codeFrom = null, int? codeTo = null, OrderBy? orderById = null)
    {
        CodeFrom = codeFrom;
        CodeTo = codeTo;
        OrderById = orderById;
    }
}

public enum OrderBy
{
    asc,
    desc
}
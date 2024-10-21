namespace FinBeatTech.Models;

public class ValueFilters
{
    public int? CodeFrom { get; private set; }
    public int? CodeTo { get; private set; }
    public OrderBy? OrderById { get; private set; }

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
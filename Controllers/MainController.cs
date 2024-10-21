using FinBeatTech.BusinessLogic;
using FinBeatTech.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FinBeatTech.Controllers;

[ApiController]
public class MainController : BaseController
{
    private readonly IMainLogic _mainLogic;
    public MainController(IMainLogic mainLogic)
    {
        _mainLogic = mainLogic;
    }

    
    /// <remarks>
    /// Example:
    /// [
    ///     {"10":"asdfasdfsadfv"},
    ///     {"1":"vav"},
    ///     {"5":"asdfasdf"},
    /// ]
    /// </remarks>
    [HttpPost]
    [Route("set")]
    public JObject PostValue(List<Dictionary<string, string>> values)
    {
        try
        {
            _mainLogic.SetNewValue(values);
            
            return new JObject()
            {
                { "success", true }
            };
        }
        catch (FormatException ex)
        {
            logger.Error($"Data parsing error!{ex.Message}");
            Response.StatusCode = 400;
            return new JObject()
            {
                { "success", false },
                { "Error", "Incorrect input data!" }
            };
        }
        catch (Exception ex)
        {
            Response.StatusCode = 400;
            return new JObject()
            {
                { "success", false },
                { "Error", ex.Message }
            };
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="countPageLim"></param>
    /// <param name="codeFrom"></param>
    /// <param name="codeTo"></param>
    /// <param name="orderById">0 - asc, 1 - desc</param>
    /// <returns></returns>
    [HttpGet]
    [Route("get")]
    public JObject GetValue(int page = 1, int countPageLim = 20, int? codeFrom = null, int? codeTo = null, OrderBy? orderById = null )
    {
        var filters = new ValueFilters(codeFrom, codeTo, orderById);
        try
        {
            var result = _mainLogic.GetValues(page, countPageLim, filters);
            
            return new JObject()
            {
                { "success", true },
                { "Total", _mainLogic.GetTotalCount() },
                { "Count", result.Count },
                { "Result", result }
            };
        }
        catch (Exception ex)
        {
            Response.StatusCode = 400;
            return new JObject()
            {
                { "success", false },
                { "Error", ex.Message }
            };
        }
    }
}
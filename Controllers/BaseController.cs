using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinBeatTech.Controllers;

[TypeFilter(typeof(LogActionAttribute))]
public class BaseController : Controller
{
    protected IConfiguration Configuration;

    protected NLog.Logger logger;

    public BaseController()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
}

/// <summary>
/// LogActionAttribute
/// </summary>
public class LogActionAttribute : ActionFilterAttribute
{
    /// <summary>
    /// OnActionExecuting
    /// </summary>       
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var controller = filterContext.RouteData.Values["Controller"];
        var action = filterContext.RouteData.Values["Action"];
        Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " " + controller + " [" + action + "]");
        base.OnActionExecuting(filterContext);
    }
}
using FinBeatTech.BusinessLogic;
using FinBeatTech.DataProcessor;
using FinBeatTech.DataProcessor.Interface;
using FinBeatTech.Service;

namespace FinBeatTech;

public class ConfigureServices
{
    public static void SetDIConfig(IServiceCollection services)
    {
        services.AddSingleton<IMainLogic, MainService>();
        services.AddSingleton<IValueDp, ValueDp>();
        /*services.AddSingleton<ICarDp, CarDp>();
        services.AddSingleton<ISellerDp, SellerDp>();
        services.AddSingleton<IBuyerDp, BuyerDp>();
        services.AddSingleton<ICar, CarService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddSingleton<IBuyer, BuyerService>();
        services.AddSingleton<ISeller, SellerService>();
        services.AddSingleton<IUser, UserService>();*/
    }
}
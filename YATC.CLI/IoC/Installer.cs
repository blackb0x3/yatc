using Microsoft.Extensions.DependencyInjection;
using YATC.Core;

namespace YATC.CLI.IoC;

public class Installer
{
    public static void Install(IServiceCollection services)
    {
        services.AddTransient<IYatcLogger, YatcLogger>();
        services.AddTransient<ITwitchAuthenticationService, TwitchAuthenticationService>();
    }
}
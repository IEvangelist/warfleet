using Microsoft.Extensions.DependencyInjection;

namespace IEvangelist.Blazing.WarFleet.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWarFleetServices(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddSingleton<GameEngineService>();
            services.AddSingleton<GameHostService>();

            return services;
        }
    }
}

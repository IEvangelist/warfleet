using Microsoft.Extensions.DependencyInjection;

namespace IEvangelist.Blazing.WarFleet.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWarFleetServices(this IServiceCollection services)
        {
            services.AddSingleton<GameEngineService>();
            services.AddSingleton<GameHostService>();

            return services;
        }
    }
}

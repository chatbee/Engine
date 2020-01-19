using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;


namespace Chatbees.Engine
{
    public static class Extension
    {
        public static IServiceCollection AddChatbeesEngine(this IServiceCollection services, MemoryCacheOptions cacheOptions)
        {
            services.AddSingleton<IEngineService>(new EngineService(cacheOptions));
            return services;
        }
    }
}

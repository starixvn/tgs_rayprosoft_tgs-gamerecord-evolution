using System.Reflection;
using StackExchange.Redis;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.EvolutionService.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameRecordEvolutionServiceCore(
            this IServiceCollection services,
            Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
            Action<GameInterfaceEvoOptions, IServiceProvider> gameInterfaceEvoOptions,
            Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver
            )
            => services
                .AddOptions<GameRecordInfoOptions>()
                .Configure(gameRecordInfoOptions)
                .Services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameInterfaceEvoClient(gameInterfaceEvoOptions)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, redisDatabaseResolver(sp)))
                .AddTransient<IUtensil, UtensilRepository>()
                .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
    }
}

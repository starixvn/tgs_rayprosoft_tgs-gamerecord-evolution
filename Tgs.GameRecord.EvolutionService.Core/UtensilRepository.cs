using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.EvolutionService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly IGameCommandClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public UtensilRepository(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameCommandClient client)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
        => _client
        .BrandGetGameProviderConfigurationIdAsync(
            brandId,
            gameProviderId,
            cancellationToken);

    public async ValueTask<int?> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default)
    {
        var gameModel = await _client
            .GameGetAsync(
                gameProviderId,
                gameExternalId,
                cancellationToken)
            .ConfigureAwait(false);
        return gameModel?.GameId;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(CancellationToken cancellationToken = default)
    => await _client
        .GameProviderGetGameProviderIdAsync(
            _gameRecordInfo.GameProviderCode,
            cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"The identifier for this game provider does not exist.({_gameRecordInfo.GameProviderCode})");

    public ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
        => _client
        .PlayerGetAsync(
            playerCode,
            cancellationToken);
}

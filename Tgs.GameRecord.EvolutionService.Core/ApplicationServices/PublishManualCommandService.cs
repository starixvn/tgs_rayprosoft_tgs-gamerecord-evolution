using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

internal class PublishManualCommandService : ICommandService<PublishManualCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<PublishManualCommandService> _logger;
    private readonly IGameRoundsWorkManualClient _manualClient;
    private readonly IUtensil _utensil;
    private readonly IGameRoundsWorkClient _workClient;

    public PublishManualCommandService(
        ILogger<PublishManualCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IUtensil utensil,
        IGameRoundsWorkManualClient manualClient,
        IGameRoundsWorkClient workClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _manualClient = manualClient ?? throw new ArgumentNullException(nameof(manualClient));
        _workClient = workClient ?? throw new ArgumentNullException(nameof(workClient));
    }

    public async ValueTask ExecuteAsync(PublishManualCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameProviderId = await _utensil
                .GetGameProviderIdAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: command.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: command.ExternalGameId.ToString(),
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        brandId: playerModel.BrandId,
                        gameProviderId: gameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            if (success)
            {
                var playerWinLoss = command.SettleAmount - command.ValidBetAmount;

                await _manualClient
                    .RoundBidding(
                        gameProviderId: gameProviderId,
                        gameProviderCode: _gameRecordInfo.GameProviderCode,
                        externalRoundId: command.ExternalRoundId,
                        gameId: gameId.Value,
                        externalGameId: command.ExternalGameId,
                        playerCode: command.PlayerCode,
                        walletId: playerModel!.DefaultWalletId,
                        currencyCode: playerModel!.DefaultWalletCurrencyCode,
                        brandId: playerModel.BrandId,
                        gameProviderConfigurationId: gameProviderConfigurationId!.Value,
                        status: (GameRounds.Work.Manual.Common.Protos.RoundStatus)RoundStatus.Closed,
                        clientStatus: (GameRounds.Work.Manual.Common.Protos.RoundClientStatus)((playerWinLoss > 0) ? RoundClientStatus.Win : RoundClientStatus.Loss),
                        externalCreatedOn: command.ExternalCreatedOn,
                        externalClosedOn: command.ExternalClosedOn,
                        bet: command.BetAmount,
                        validBet: command.ValidBetAmount,
                        settle: command.SettleAmount,
                        commission: 0,
                        turnover: command.ValidBetAmount,
                        playerWinLoss: playerWinLoss,
                        additionalData: command.AdditionalData,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await _workClient
                    .RoundMissing(
                        gameProviderId: gameProviderId,
                        gameProviderCode: _gameRecordInfo.GameProviderCode,
                        externalRoundId: command.ExternalRoundId,
                        gameId: gameId ?? -1,
                        externalGameId: command.ExternalGameId,
                        playerCode: command.PlayerCode,
                        data: JsonSerializer.Serialize(command),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "遊戲紀錄處理發生例外.({command})", command);
        }
    }
}

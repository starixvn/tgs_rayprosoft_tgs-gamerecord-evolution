using System.Text.Json;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Manual.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

internal class ManualHandleRecordCommandService : ICommandService<ManualHandleRecordCommand>
{
    private readonly IGameRoundsWorkManualClient _client;
    private readonly IUtensil _utensil;

    public ManualHandleRecordCommandService(
        IUtensil utensil,
        IGameRoundsWorkManualClient client)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(ManualHandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;
        var playerModel = await _utensil
            .GetPlayerAsync(
                playerCode: command.Record.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: command.Record.ExternalGameId.ToString(),
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
            var status = RoundStatus.Closed;
            var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
            var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
            var bet = command.Record.BetAmount.ToDecimal().GetValueOrDefault(0);
            var validBet = command.Record.ValidBetAmount.ToDecimal().GetValueOrDefault(0);
            var settle = command.Record.SettleAmount.ToDecimal().GetValueOrDefault(0);
            var commission = 0;
            var turnover = command.Record.ValidBetAmount.ToDecimal().GetValueOrDefault(0);
            var winLoss = settle - validBet;
            var clientStatus = winLoss <= 0 ? RoundClientStatus.Loss : RoundClientStatus.Win;

            await _client
                .RoundBidding(
                    gameProviderId: gameProviderId,
                    gameProviderCode: gameProviderCode,
                    externalRoundId: command.Record.ExternalRoundId,
                    gameId: gameId!.Value,
                    externalGameId: command.Record.ExternalGameId,
                    playerCode: playerModel!.PlayerCode,
                    walletId: playerModel!.DefaultWalletId,
                    currencyCode: playerModel!.DefaultWalletCurrencyCode,
                    brandId: playerModel!.BrandId,
                    gameProviderConfigurationId: gameProviderConfigurationId!.Value,
                    status: status,
                    clientStatus: clientStatus,
                    externalCreatedOn: externalCreatedOn,
                    externalClosedOn: externalClosedOn,
                    bet: bet,
                    validBet: validBet,
                    settle: settle,
                    commission: commission,
                    turnover: turnover,
                    playerWinLoss: winLoss,
                    additionalData: command.Record.AdditionalData,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            await _client
                .RoundMissing(
                    gameProviderId: gameProviderId,
                    gameProviderCode: gameProviderCode,
                    externalRoundId: command.Record.ExternalRoundId,
                    gameId: gameId ?? -1,
                    externalGameId: command.Record.ExternalGameId,
                    playerCode: playerModel?.PlayerCode ?? string.Empty,
                    data: JsonSerializer.Serialize(command.Record),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

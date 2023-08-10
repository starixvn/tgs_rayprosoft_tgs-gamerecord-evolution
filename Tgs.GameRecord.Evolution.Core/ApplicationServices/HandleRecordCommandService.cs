using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRoundsWorkClient _client;
    private readonly IUtensil _utensil;

    public HandleRecordCommandService(
        IUtensil utensil,
        IGameRoundsWorkClient client)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil.GetGameProviderCode();

        var playerModel = await _utensil
            .GetPlayerAsync(
            playerCode: command.Record.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameId = await _utensil
            .GetGameIdAsync(
            gameProviderId: gameProviderId,
            gameExternalId: command.Record.ExternalGameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _utensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: playerModel.BrandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var status = RoundStatus.Closed;
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
        var commission = command.Record.Commission.ToDecimal().GetValueOrDefault(0);
        var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
        var winLoss = command.Record.WinLoss.ToDecimal().GetValueOrDefault(0);
        var clientStatus = command.Record.IsVoid || command.Record.IsCancelled
            ? RoundClientStatus.Void
            : winLoss > 0 ? RoundClientStatus.Win : RoundClientStatus.Loss;

        await _client
            .RoundExchangeRateBiddingAsync(
            gameProviderId: gameProviderId,
            gameProviderCode: gameProviderCode,
            externalRoundId: command.Record.ExternalRoundId,
            gameId: gameId,
            externalGameId: command.Record.ExternalGameId,
            playerCode: playerModel!.PlayerCode,
            walletId: playerModel!.DefaultWalletId,
            currencyCode: playerModel!.DefaultWalletCurrencyCode,
            brandId: playerModel!.BrandId,
            gameProviderConfigurationId: gameProviderConfigurationId,
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
}

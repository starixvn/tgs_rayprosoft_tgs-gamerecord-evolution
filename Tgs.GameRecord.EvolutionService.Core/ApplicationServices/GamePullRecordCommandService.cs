using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _gameRecordInfo = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        foreach (var record in command.Records)
        {
            bool success = true;
            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: record.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: record.ExternalGameId.ToString(),
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

            var playerWinLoss = record.SettleAmount.ToDecimal().GetValueOrDefault(0) - record.ValidBetAmount.ToDecimal().GetValueOrDefault(0);

            var message = new GameRecordMessage(
                ExternalRoundId: record.ExternalRoundId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.ExternalGameId.ToString(),
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                Status: GameRoundStatus.Closed,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                ExternalCreatedOn: record.ExternalCreatedOn.ToDateTimeOffset(),
                ExternalClosedOn: record.ExternalClosedOn.ToDateTimeOffset(),
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                BetAmount: record.BetAmount.ToDecimal().GetValueOrDefault(0),
                ValidBetAmount: record.ValidBetAmount.ToDecimal().GetValueOrDefault(0),
                SettleAmount: record.SettleAmount.ToDecimal().GetValueOrDefault(0),
                Commission: 0,
                Turnover: record.ValidBetAmount.ToDecimal().GetValueOrDefault(0),
                PlayerWinLoss: playerWinLoss,
                AdditionalData: record.AdditionalData,
                ClientStatus: (playerWinLoss > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss,
                Success: success);

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{message}",
                DateTime.Now.ToString("o"),
                "tgs.gp.gamerecord.database.round.evotf",
                message);

            await _messageSender
                .PublishAsync(
                    subject: "tgs.gp.gamerecord.database.round.evotf",
                    data: message.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

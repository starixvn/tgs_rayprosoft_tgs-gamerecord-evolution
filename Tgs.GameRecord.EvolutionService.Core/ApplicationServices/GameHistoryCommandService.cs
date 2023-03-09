using Microsoft.Extensions.Logging;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices;

internal class GameHistoryCommandService : ICommandService<GameHistoryCommand>
{
    private readonly IGameInterfaceEvolutionClient _client;
    private readonly ILogger<GameHistoryCommandService> _logger;
    private readonly ICommandService<PublishCommand> _service;

    public GameHistoryCommandService(
        ILogger<GameHistoryCommandService> logger,
        IGameInterfaceEvolutionClient client,
        ICommandService<PublishCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(GameHistoryCommand command, CancellationToken cancellationToken = default)
    {
        var gameHistoriesReply = await _client
            .GetGameHistoriesAsync(
                apiUrl: command.ApiUrl,
                casinoId: command.CasinoId,
                gameHistoryApiToken: command.GameHistoryApiToken,
                startDate: command.StartDate,
                endDate: command.EndDate,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
            DateTime.UtcNow.ToString("O"),
            command,
            gameHistoriesReply);

        if (gameHistoriesReply == null)
        {
            return;
        }

        var records = gameHistoriesReply.Records ?? Enumerable.Empty<GameHistoryRecord>();

        if (!records.Any())
        {
            return;
        }

        foreach (var record in records)
        {
            var publishCommand = new PublishCommand(
                    record.PlayerCode,
                    record.ExternalRoundId,
                    record.ExternalGameId,
                    record.ExternalCreatedOn.ToDateTimeOffset(),
                    record.ExternalClosedOn.ToDateTimeOffset(),
                    record.BetAmount.ToDecimal().GetValueOrDefault(0),
                    record.ValidBetAmount.ToDecimal().GetValueOrDefault(0),
                    record.SettleAmount.ToDecimal().GetValueOrDefault(0),
                    record.AdditionalData);

            await _service
                 .ExecuteAsync(
                     command: publishCommand,
                     cancellationToken: cancellationToken)
                 .ConfigureAwait(false);
        }
    }
}

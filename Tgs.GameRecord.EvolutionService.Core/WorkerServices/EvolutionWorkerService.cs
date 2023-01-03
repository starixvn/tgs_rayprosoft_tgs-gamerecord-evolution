using Microsoft.Extensions.Logging;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices;

internal class EvolutionWorkerService : ICommandService<EvolutionWorkerCommand>
{
    private readonly IGameInterfaceEvolutionClient _client;
    private readonly ILogger<EvolutionWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _service;

    public EvolutionWorkerService(
        IGameInterfaceEvolutionClient client,
        ILogger<EvolutionWorkerService> logger,
        ICommandService<GamePullRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(EvolutionWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameHistoriesReply = await _client.GetGameHistoriesAsync(
           apiUrl: command.GameApiUrl,
           casinoId: command.CasinoId,
           gameHistoryApiToken: command.GameHistoryApiToken,
           startDate: command.StartDate,
           endDate: command.EndDate,
           cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
            DateTime.UtcNow.ToString("O"),
            command,
            gameHistoriesReply);

        if (gameHistoriesReply == null)
        {
            return;
        }

        if (gameHistoriesReply.Records == null || !gameHistoriesReply.Records.Any())
        {
            return;
        }

        await _service.ExecuteAsync(
            command: new GamePullRecordCommand(Records: gameHistoriesReply.Records),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

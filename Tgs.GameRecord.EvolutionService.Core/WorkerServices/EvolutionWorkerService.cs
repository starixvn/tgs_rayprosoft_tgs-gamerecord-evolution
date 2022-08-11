using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices;

internal class EvolutionWorkerService : ICommandService<EvolutionWorkerCommand>
{
    private readonly IGameInterfaceEvolutionClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<EvolutionWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _service;
    private readonly IUtensil _utensil;

    public EvolutionWorkerService(
        IOptions<GameRecordInfoOptions> options,
        IGameInterfaceEvolutionClient client,
        ILogger<EvolutionWorkerService> logger,
        IUtensil utensil,
        ICommandService<GamePullRecordCommand> service)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(EvolutionWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false);

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

        await _service.ExecuteAsync(new GamePullRecordCommand(
            GameProviderId: gameProviderId,
            GameProviderCode: _gameRecordInfo.GameProviderCode,
            Records: gameHistoriesReply.Records), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

using Microsoft.Extensions.Logging;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly IGameInterfaceEvolutionClient _client;
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        IGameInterfaceEvolutionClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameRecordInfos = await _client
            .GameRecordAsync(
            apiUrl: command.ApiUrl,
            casinoId: command.CasinoId,
            gameHistoryApiToken: command.GameHistoryApiToken,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameRecordInfo>();

        _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
            DateTime.UtcNow.ToString("O"),
            command,
            gameRecordInfos);

        if (!gameRecordInfos.Any())
        {
            return;
        }

        foreach (var gameRecordInfo in gameRecordInfos)
        {
            try
            {
                await _service
                    .ExecuteAsync(
                    command: new(gameRecordInfo),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "遊戲紀錄處理發生例外.({gameRecord})", gameRecordInfo);
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Core.ApplicationServices;

internal class PullRecordCommandService(
    ILogger<PullRecordCommandService> logger,
    IGameInterfaceEvolutionClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameRecordInfos = await client
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

        logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
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
                await service
                    .ExecuteAsync(
                    command: new(gameRecordInfo),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "遊戲紀錄處理發生例外.({gameRecord})", gameRecordInfo);
            }
        }
    }
}

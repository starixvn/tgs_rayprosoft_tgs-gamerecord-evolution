using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core;

internal class ScheduleFormDispatcher : IScheduleFormDispatcher
{
    private readonly IGameRoundsScheduleClient _clinet;
    private readonly ILogger<ScheduleFormDispatcher> _logger;
    private readonly ICommandService<GameHistoryManualCommand> _manualService;
    private readonly IScheduleFormLock _scheduleFormLock;
    private readonly ICommandService<GameHistoryCommand> _service;

    public ScheduleFormDispatcher(
        ILogger<ScheduleFormDispatcher> logger,
        IGameRoundsScheduleClient clinet,
        IScheduleFormLock scheduleFormLock,
        ICommandService<GameHistoryCommand> service,
        ICommandService<GameHistoryManualCommand> manualService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clinet = clinet ?? throw new ArgumentNullException(nameof(clinet));
        _scheduleFormLock = scheduleFormLock ?? throw new ArgumentNullException(nameof(scheduleFormLock));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _manualService = manualService ?? throw new ArgumentNullException(nameof(manualService));
    }

    public async ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default)
    {
        var scheduleForm = await _clinet
            .ScheduleFormGetAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"The game provider ({gameProviderCode}) cannot find the schedule form. ({id})");

        if (scheduleForm.State != ScheduleFormState.Queued)
        {
            _logger.LogWarning("This game provider ({gameProviderCode}) schedule form status is not queued.({id})", gameProviderCode, id);
            return;
        }

        if (!await _scheduleFormLock
            .LockingAsync(
                gameProviderCode: gameProviderCode,
                id: id)
            .ConfigureAwait(false))
        {
            _logger.LogWarning("LogOn:{logOn} Message:This game provider ({gameProviderCode}) schedule form has been locked. ({id})",
                DateTime.Now.ToString("o"),
                gameProviderCode,
                id);
            return;
        }

        try
        {
            await _clinet
                .ScheduleFormProcessingAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var parameter = JsonSerializer.Deserialize<EvolutionParameter>(scheduleForm.Parameters)
                ?? throw new Exception($"This game provider ({gameProviderCode}) scheduling form parameter is null. ({id})");

            if (scheduleForm.Source == ScheduleFormSource.Schedule)
            {
                var command = new GameHistoryCommand(
                        ApiUrl: new Uri(parameter.ApiUrl),
                        CasinoId: parameter.CasinoId,
                        GameHistoryApiToken: parameter.GameHistoryApiToken,
                        StartDate: parameter.StartTime.ToDateTimeOffset(),
                        EndDate: parameter.EndTime.ToDateTimeOffset());

                await _service
                     .ExecuteAsync(
                         command: command,
                         cancellationToken: cancellationToken)
                     .ConfigureAwait(false);
            }
            else
            {
                var command = new GameHistoryManualCommand(
                        ApiUrl: new Uri(parameter.ApiUrl),
                        CasinoId: parameter.CasinoId,
                        GameHistoryApiToken: parameter.GameHistoryApiToken,
                        StartDate: parameter.StartTime.ToDateTimeOffset(),
                        EndDate: parameter.EndTime.ToDateTimeOffset());

                await _manualService
                     .ExecuteAsync(
                         command: command,
                         cancellationToken: cancellationToken)
                     .ConfigureAwait(false);
            }

            await _clinet
                .ScheduleFormCompleteAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in the game provider ({gameProviderCode}) schedule form. ({id})", gameProviderCode, id);

            await _clinet
                .ScheduleFormFaileAsync(
                    gameProviderCode: gameProviderCode,
                    id: id,
                    status: ex.Message,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _ = await _scheduleFormLock
                 .UnlockAsync(
                     gameProviderCode: gameProviderCode,
                     id: id)
                 .ConfigureAwait(false);
        }
    }
}

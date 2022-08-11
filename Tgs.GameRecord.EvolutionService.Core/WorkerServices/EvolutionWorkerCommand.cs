namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices;

public record EvolutionWorkerCommand(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record SchedulePullRecordCommand(
    Uri ApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

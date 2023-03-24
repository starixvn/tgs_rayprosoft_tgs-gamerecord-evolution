namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record ManualPullRecordCommand(
    Uri ApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

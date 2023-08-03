namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);

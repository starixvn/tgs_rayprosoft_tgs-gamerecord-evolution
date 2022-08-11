namespace Tgs.GameRecord.EvolutionService;

public record ScheduleMessageModel(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken);

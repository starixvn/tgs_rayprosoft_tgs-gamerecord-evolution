namespace Tgs.GameRecord.EvolutionService.Messages;

public record ScheduleMessageModel(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken);

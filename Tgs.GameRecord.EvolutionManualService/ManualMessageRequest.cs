namespace Tgs.GameRecord.EvolutionManualService;

public record ManualMessageRequest(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

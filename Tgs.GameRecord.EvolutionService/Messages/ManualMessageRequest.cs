namespace Tgs.GameRecord.EvolutionService.Messages;

public record ManualMessageRequest(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

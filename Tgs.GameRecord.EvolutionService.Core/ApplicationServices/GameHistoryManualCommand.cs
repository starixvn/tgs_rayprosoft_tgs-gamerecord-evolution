namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record GameHistoryManualCommand(
    Uri ApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

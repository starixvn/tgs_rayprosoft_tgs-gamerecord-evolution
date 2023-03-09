namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record GameHistoryCommand(
    Uri ApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

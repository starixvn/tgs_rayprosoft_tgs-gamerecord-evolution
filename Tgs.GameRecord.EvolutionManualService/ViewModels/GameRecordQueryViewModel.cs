namespace Tgs.GameRecord.EvolutionManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);

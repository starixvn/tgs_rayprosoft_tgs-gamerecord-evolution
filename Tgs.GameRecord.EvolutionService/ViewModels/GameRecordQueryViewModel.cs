namespace Tgs.GameRecord.JiliService.ViewModels
{
    public record GameRecordQueryViewModel(
        Uri GameApiUrl,
        string CasinoId,
        string GameHistoryApiToken,
        DateTimeOffset StartDate,
        DateTimeOffset EndDate);
}

namespace Tgs.GameRecord.JiliService.ViewModels
{
    public record GameRecordQueryViewModel(
        Uri GameApiUrl,
        string Authorization,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}

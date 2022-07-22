namespace Tgs.GameRecord.EvolutionService
{
    public record ManualMessageRequest(
        Uri GameApiUrl,
        string Authorization,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}

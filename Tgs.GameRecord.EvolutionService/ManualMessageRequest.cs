namespace Tgs.GameRecord.EvolutionService
{
    public record ManualMessageRequest(
        Uri GameApiUrl,
        string CasinoId,
        string GameHistoryApiToken,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}

namespace Tgs.GameRecord.EvolutionService.Core.DispatchServices
{
    public record ScheduleDispatchCommand(
        Uri GameApiUrl,
        string CasinoId,
        string GameHistoryApiToken);
}

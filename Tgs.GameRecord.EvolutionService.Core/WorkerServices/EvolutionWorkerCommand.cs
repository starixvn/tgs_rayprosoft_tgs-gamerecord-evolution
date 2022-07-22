namespace Tgs.GameRecord.EvolutionService.Core.WorkerServices
{
    public record EvolutionWorkerCommand(
        Uri GameApiUrl,
        string Authorization,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}

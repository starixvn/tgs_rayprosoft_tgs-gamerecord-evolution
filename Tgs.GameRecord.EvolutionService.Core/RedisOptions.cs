namespace Tgs.GameRecord.EvolutionService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}

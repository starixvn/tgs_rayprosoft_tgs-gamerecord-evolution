namespace Tgs.GameRecord.Evolution.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}

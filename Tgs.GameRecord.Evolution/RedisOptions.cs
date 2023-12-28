using Tgs.GameRecord.Evolution.Core;
namespace Tgs.GameRecord.Evolution;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}

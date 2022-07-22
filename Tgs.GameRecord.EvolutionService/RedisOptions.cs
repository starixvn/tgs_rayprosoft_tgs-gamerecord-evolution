namespace Tgs.GameRecord.EvolutionService
{
    public record RedisOptions
    {
        public string ConnectionString { get; set; } = "localhost";
    }
}

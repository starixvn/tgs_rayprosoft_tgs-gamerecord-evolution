namespace Tgs.GameRecord.EvolutionService.Core
{
    public record GameRecordInfoOptions()
    {
        public string GameProviderCode { get; set; } = default!;
        public decimal IntervalMinute { get; set; } = 5m;
    }
}

namespace Tgs.GameRecord.EvolutionService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; set; } = default!;
    public string GameProviderHangfire { get; set; } = default!;
    public double IntervalMinutes { get; set; } = 3;
    public double IntervalMinutesMax { get; set; } = 60d;
    public double IntervalMinutesMinimum { get; set; } = 1;
}

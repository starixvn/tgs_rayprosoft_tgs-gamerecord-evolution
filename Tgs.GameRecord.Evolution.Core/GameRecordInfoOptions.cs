namespace Tgs.GameRecord.Evolution.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; set; } = default!;
	public string HangfirePrefix { get; set; } = "evotf-schedule:hangfire:";
	public string HangfireLocking { get; set; } = "evotf-locking:hangfire:";
	public double IntervalMinutes { get; set; } = 3;
	public double IntervalMinutesMax { get; set; } = 60d;
	public double IntervalMinutesMinimum { get; set; } = 1;
}

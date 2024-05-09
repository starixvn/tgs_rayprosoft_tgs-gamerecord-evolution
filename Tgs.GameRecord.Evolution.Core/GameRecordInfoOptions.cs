namespace Tgs.GameRecord.Evolution.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; set; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-evotf-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-evotf-locking:";
	public double IntervalMinutes { get; set; } = 3;
	public double IntervalMinutesMax { get; set; } = 60d;
	public double IntervalMinutesMinimum { get; set; } = 1;
}

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Evolution.Common;

public record EvolutionGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string CasinoId { get; init; } = default!;
	[Required]
	public string GameHistoryApiToken { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}

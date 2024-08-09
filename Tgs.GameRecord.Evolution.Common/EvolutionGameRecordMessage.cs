using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Evolution.Common;

public record EvolutionGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://babylonpega2.uat1.evo-test.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "babylonpega20001")]
	public string CasinoId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "test123")]
	public string GameHistoryApiToken { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-09-07T18:50:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2022-09-07T18:55:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}

namespace Tgs.GameRecord.Evolution.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string CasinoId,
	string GameHistoryApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);

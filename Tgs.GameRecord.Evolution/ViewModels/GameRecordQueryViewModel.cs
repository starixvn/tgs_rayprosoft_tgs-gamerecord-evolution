namespace Tgs.GameRecord.Evolution.ViewModels;

public record GameRecordQueryViewModel(
	Uri GameApiUrl,
	string CasinoId,
	string GameHistoryApiToken,
	DateTimeOffset StartDate,
	DateTimeOffset EndDate);

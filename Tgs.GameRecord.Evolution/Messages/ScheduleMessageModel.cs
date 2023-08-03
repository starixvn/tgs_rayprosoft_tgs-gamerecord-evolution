namespace Tgs.GameRecord.Evolution.Messages;

public record ScheduleMessageModel(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken);

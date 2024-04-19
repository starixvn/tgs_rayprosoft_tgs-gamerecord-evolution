namespace Tgs.GameRecord.Evolution.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string CasinoId,
	string GameHistoryApiToken);

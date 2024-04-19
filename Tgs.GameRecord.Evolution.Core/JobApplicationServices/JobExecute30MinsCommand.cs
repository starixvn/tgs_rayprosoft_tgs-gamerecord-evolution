namespace Tgs.GameRecord.Evolution.Core.JobApplicationServices;

public record JobExecute30MinsCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string CasinoId,
	string GameHistoryApiToken);

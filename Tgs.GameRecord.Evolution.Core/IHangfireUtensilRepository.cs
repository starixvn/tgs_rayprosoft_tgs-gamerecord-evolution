using Tgs.GameRecord.Evolution.Core.JobApplicationServices;

namespace Tgs.GameRecord.Evolution.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobExecute30MinsAsync(JobExecute30MinsCommand source);

	public Task JobInitAsync();
}

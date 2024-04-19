using Tgs.GameRecord.Evolution.Core.JobApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Core;

internal class HangfireUtensil(
	ICommandService<JobCheckCommand> jobCheckService,
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobExecute30MinsCommand> jobExecute30MinsService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensil
{
	public Task JobCheckAsync()
		=> jobCheckService.ExecuteAsync(new()).AsTask();
	public Task JobExecute30MinsAsync(JobExecute30MinsCommand source)
		 => jobExecute30MinsService.ExecuteAsync(source).AsTask();
	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}

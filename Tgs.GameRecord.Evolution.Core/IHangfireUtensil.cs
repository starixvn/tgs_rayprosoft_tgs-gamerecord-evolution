using Tgs.GameRecord.Evolution.Core.JobApplicationServices;

namespace Tgs.GameRecord.Evolution.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}

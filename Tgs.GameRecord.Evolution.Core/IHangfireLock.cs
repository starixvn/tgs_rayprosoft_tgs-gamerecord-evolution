namespace Tgs.GameRecord.Evolution.Core;

public interface IHangfireLock
{
	public Task<bool> LockingAsync(string application, CancellationToken cancellationToken = default);
}

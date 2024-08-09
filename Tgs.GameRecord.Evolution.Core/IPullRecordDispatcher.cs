using Tgs.GameRecord.Evolution.Core.ApplicationServices;

namespace Tgs.GameRecord.Evolution.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}

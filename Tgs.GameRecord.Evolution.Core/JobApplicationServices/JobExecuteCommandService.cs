using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Evolution.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Evolution.Core.JobApplicationServices;

internal class JobExecuteCommandService(
		ILogger<JobExecuteCommandService> logger,
		IOptions<GameRecordInfoOptions> options,
		TimeProvider timeProvider,
		IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	private readonly double _intervalMinutes = Math.Clamp(
		value: options.Value.IntervalMinutes,
		min: options.Value.IntervalMinutesMinimum,
		max: options.Value.IntervalMinutesMax);

	public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 00, currentTime.Offset);
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var parameters = new
		{
			command.ApiUrl,
			command.CasinoId,
			command.GameHistoryApiToken,
			StartTime = startTime,
			EndTime = endTime
		};

		logger.LogInformation("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

		return messageSender
			.PublishAsync(
			subject: EvolutionNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(parameters),
			cancellationToken: cancellationToken);
	}
}

using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Evolution.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Evolution.Core.JobApplicationServices;

internal class JobExecute30MinsCommandService : ICommandService<JobExecute30MinsCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo;
	private readonly double _intervalMinutes;
	private readonly ILogger<JobExecute30MinsCommandService> _logger;
	private readonly TimeProvider _timeProvider;
	private readonly IMessageSender _messageSender;

	public JobExecute30MinsCommandService(
		ILogger<JobExecute30MinsCommandService> logger,
		IOptions<GameRecordInfoOptions> options,
		TimeProvider timeProvider,
		IMessageSender messageSender)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
		_messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
		_gameRecordInfo = options.Value;
		_intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
			? _gameRecordInfo.IntervalMinutesMax
			: _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
			? _gameRecordInfo.IntervalMinutesMinimum
			: _gameRecordInfo.IntervalMinutes;
	}

	public ValueTask ExecuteAsync(JobExecute30MinsCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = _timeProvider.GetUtcNow();
		var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 00, currentTime.Offset).AddMinutes(-30);
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var parameters = new
		{
			command.ApiUrl,
			command.CasinoId,
			command.GameHistoryApiToken,
			StartTime = startTime,
			EndTime = endTime
		};

		_logger.LogInformation("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

		return _messageSender
			.PublishAsync(
			subject: EvolutionNatsConst.ScheduleMinsPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(parameters),
			cancellationToken: cancellationToken);
	}
}

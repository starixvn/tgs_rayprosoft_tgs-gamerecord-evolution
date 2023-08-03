using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Evolution.Core;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Messages;

internal class ScheduleMessageHandler : IMessageHandler
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinutes;
    private readonly ILogger<ScheduleMessageHandler> _logger;
    private readonly ICommandService<PullRecordCommand> _service;

    public ScheduleMessageHandler(
        ILogger<ScheduleMessageHandler> logger,
        IOptions<GameRecordInfoOptions> options,
        ICommandService<PullRecordCommand> service
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMax
            : _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMinimum
            : _gameRecordInfo.IntervalMinutes;
    }

    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var messageModel = JsonSerializer.Deserialize<ScheduleMessageModel>(json);

        if (messageModel == null)
        {
            _logger.LogError("Information Received:{LogInfo}", messageModel);
            return ValueTask.CompletedTask;
        }

        var endDate = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

        var startDate = endDate.AddMinutes(_intervalMinutes * -1);

        _logger.LogInformation("Information Received:{LogInfo}", messageModel);

        var command = new PullRecordCommand(
            ApiUrl: messageModel.GameApiUrl,
            CasinoId: messageModel.CasinoId,
            GameHistoryApiToken: messageModel.GameHistoryApiToken,
            StartTime: startDate,
            EndTime: endDate);

        return _service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}

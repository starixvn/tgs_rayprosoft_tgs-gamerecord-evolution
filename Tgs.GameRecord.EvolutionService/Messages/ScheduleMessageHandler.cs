using System.Text;
using System.Text.Json;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Messages;

internal class ScheduleMessageHandler : IMessageHandler
{
    private readonly ILogger<ScheduleMessageHandler> _logger;
    private readonly ICommandService<SchedulePullRecordCommand> _service;

    public ScheduleMessageHandler(
        ILogger<ScheduleMessageHandler> logger,
        ICommandService<SchedulePullRecordCommand> service
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
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

        var startDate = endDate.AddMinutes(7 * -1);

        _logger.LogInformation("Information Received:{LogInfo}", messageModel);

        return _service.ExecuteAsync(
            command: new SchedulePullRecordCommand(
                ApiUrl: messageModel.GameApiUrl,
                CasinoId: messageModel.CasinoId,
                GameHistoryApiToken: messageModel.GameHistoryApiToken,
                StartDate: startDate,
                EndDate: endDate),
            cancellationToken: cancellationToken);
    }
}

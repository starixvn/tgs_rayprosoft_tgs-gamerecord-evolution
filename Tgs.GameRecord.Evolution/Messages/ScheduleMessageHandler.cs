using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Evolution.Common;
using Tgs.GameRecord.Evolution.Core;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Evolution.Messages;

internal class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var message = JsonSerializer.Deserialize<EvolutionGameRecordMessage>(data.Span)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.CasinoId,
					message.GameHistoryApiToken,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordCommand(
				ApiUrl: message.ApiUrl,
				CasinoId: message.CasinoId,
				GameHistoryApiToken: message.GameHistoryApiToken,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}

using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Evolution.Common;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Evolution.Messages;

internal class ManualMessageHandler(
	ILogger<ManualMessageHandler> logger,
	ICommandService<PullRecordCommand> service) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<EvolutionGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

		var command = new PullRecordCommand(
			ApiUrl: message.ApiUrl,
			CasinoId: message.CasinoId,
			GameHistoryApiToken: message.GameHistoryApiToken,
			StartTime: message.StartTime,
			EndTime: message.EndTime);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}

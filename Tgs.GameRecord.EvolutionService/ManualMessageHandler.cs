using System.Text;
using System.Text.Json;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService
{
    public class ManualMessageHandler : IMessageProcessor
    {
        private readonly ICommandService<EvolutionWorkerCommand> _service;

        public ManualMessageHandler(ICommandService<EvolutionWorkerCommand> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data.Span);
            var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

            var gameRecordCommand = new EvolutionWorkerCommand(
                GameApiUrl: request.GameApiUrl,
                CasinoId: request.CasinoId,
                GameHistoryApiToken: request.GameHistoryApiToken,
                StartDate: request.StartDate,
                EndDate: request.EndDate);

            await _service.ExecuteAsync(
                command: gameRecordCommand,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var response = new ManualMessageResponse(true);
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        }
    }
}

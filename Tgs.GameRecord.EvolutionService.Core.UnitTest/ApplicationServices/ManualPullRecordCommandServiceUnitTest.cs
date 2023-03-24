using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.UnitTest.ApplicationServices;

public class ManualPullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<ManualPullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakeService = Substitute.For<ICommandService<ManualHandleRecordCommand>>();

        var command = new ManualPullRecordCommand(
            ApiUrl: new("http://localhost/"),
            CasinoId: "casinoId",
            GameHistoryApiToken: "gameHistoryApiToken",
            StartDate: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndDate: DateTimeOffset.UtcNow);

        var service = new ManualPullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GetGameHistoriesAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                casinoId: Arg.Is(command.CasinoId),
                gameHistoryApiToken: Arg.Is(command.GameHistoryApiToken),
                startDate: Arg.Is(command.StartDate),
                endDate: Arg.Is(command.EndDate),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}

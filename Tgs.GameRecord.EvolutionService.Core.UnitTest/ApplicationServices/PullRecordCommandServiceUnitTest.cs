using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.EvolutionService.Client;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceEvolutionClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordCommand(
            ApiUrl: new("http://localhost/"),
            CasinoId: "casinoId",
            GameHistoryApiToken: "gameHistoryApiToken",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        var service = new PullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameRecordAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            casinoId: Arg.Is(command.CasinoId),
            gameHistoryApiToken: Arg.Is(command.GameHistoryApiToken),
            startTime: Arg.Is(command.StartTime),
            endTime: Arg.Is(command.EndTime),
            cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync()
            .ConfigureAwait(false);
    }
}

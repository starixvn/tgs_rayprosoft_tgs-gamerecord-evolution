using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRecord.Evolution.Core;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.UnitTest;

public class ScheduleFormDispatcherUnitTest
{
    [Fact]
    public async Task Locking()
    {
        var fakelogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeService = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "EVOTF";
        var id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var parameters = new EvolutionParameter
        {
            ApiUrl = "https://babylonpega2.uat1.evo-test.com/",
            CasinoId = "babylonpega20001",
            GameHistoryApiToken = "test123",
            StartTime = DateTimeOffset.Parse("2023-03-09T11:00:00+00:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-03-09T12:00:00+00:00").ToTimestamp(),
        };
        var scheduleForm = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "Config",
            Subject = "gp.gamerecord.scheduleform.evotf.queued",
            Parameters = JsonSerializer.Serialize(parameters),
            Source = ScheduleFormSource.Schedule,
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleForm);

        _ = fakeScheduleFormLock
            .LockingAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id))
            .Returns(false);
        var service = new ScheduleFormDispatcher(
            fakelogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeService);

        await service
            .ExecuteAsync(
                gameProviderCode,
                id);

        await fakeClient
            .Received(0)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<PullRecordCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakelogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeService = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "EVOTF";
        var id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var parameters = new EvolutionParameter
        {
            ApiUrl = "https://babylonpega2.uat1.evo-test.com/",
            CasinoId = "babylonpega20001",
            GameHistoryApiToken = "test123",
            StartTime = DateTimeOffset.Parse("2023-03-09T11:00:00+00:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-03-09T12:00:00+00:00").ToTimestamp(),
        };
        var scheduleForm = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "Config",
            Subject = "gp.gamerecord.scheduleform.evotf.queued",
            Parameters = JsonSerializer.Serialize(parameters),
            Source = ScheduleFormSource.Schedule,
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleForm);

        _ = fakeScheduleFormLock
            .LockingAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id))
            .Returns(true);
        var service = new ScheduleFormDispatcher(
            fakelogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeService);

        await service
            .ExecuteAsync(
                gameProviderCode,
                id);

        await fakeClient
            .Received(1)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Any<PullRecordCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Schedule_Form_State_Is_Not_Queued()
    {
        var fakelogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeService = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "EVOTF";
        var id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var parameters = new EvolutionParameter
        {
            ApiUrl = "https://babylonpega2.uat1.evo-test.com/",
            CasinoId = "babylonpega20001",
            GameHistoryApiToken = "test123",
            StartTime = DateTimeOffset.Parse("2023-03-09T11:00:00+00:00").ToTimestamp(),
            EndTime = DateTimeOffset.Parse("2023-03-09T12:00:00+00:00").ToTimestamp(),
        };
        var scheduleForm = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "Config",
            Subject = "gp.gamerecord.scheduleform.evotf.queued",
            Parameters = JsonSerializer.Serialize(parameters),
            Source = ScheduleFormSource.Schedule,
            State = ScheduleFormState.Processing,
            Status = $"{ScheduleFormState.Processing}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleForm);

        var service = new ScheduleFormDispatcher(
            fakelogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeService);

        await service
            .ExecuteAsync(
                gameProviderCode,
                id);

        _ = await fakeScheduleFormLock
            .Received(0)
            .LockingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id));

        await fakeClient
            .Received(0)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<PullRecordCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}

using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameRecord.Evolution.Core;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.EvolutionService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Cancelled()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "EVOTF",
            GameProviderConfiguration = "EVOTFConfiguration",
            GameProviderHangfire = "evotf-schedule:hangfire:",
            IntervalMinutes = 10,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "1776ae69497e716d7b64c0cf",
                ExternalGameId = "BacBo00000000001",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsCancelled = true,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-30T15:21:17.352Z").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-30T15:21:46.138Z").ToTimestamp(),
                Bet = 0m,
                ValidBet = 0m,
                Settle = 0m,
                Commission = 0m,
                Turnover = 0m,
                WinLoss = 0m,
                AdditionalData = new JsonObject
                {
                    ["id"] = "1776ae69497e716d7b64c0cf"
                }.ToJsonString()
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Void),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Loss()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "EVOTF",
            GameProviderConfiguration = "EVOTFConfiguration",
            GameProviderHangfire = "evotf-schedule:hangfire:",
            IntervalMinutes = 10,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "1776ae69497e716d7b64c0cf",
                ExternalGameId = "BacBo00000000001",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsCancelled = false,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-30T15:21:17.352Z").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-30T15:21:46.138Z").ToTimestamp(),
                Bet = 25m,
                ValidBet = 25m,
                Settle = 0m,
                Commission = 0m,
                Turnover = 25m,
                WinLoss = -25m,
                AdditionalData = new JsonObject
                {
                    ["id"] = "1776ae69497e716d7b64c0cf"
                }.ToJsonString()
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Loss),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Void()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "EVOTF",
            GameProviderConfiguration = "EVOTFConfiguration",
            GameProviderHangfire = "evotf-schedule:hangfire:",
            IntervalMinutes = 10,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "1776ae69497e716d7b64c0cf",
                ExternalGameId = "BacBo00000000001",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsCancelled = false,
                IsVoid = true,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-30T15:21:17.352Z").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-30T15:21:46.138Z").ToTimestamp(),
                Bet = 0m,
                ValidBet = 0m,
                Settle = 0m,
                Commission = 0m,
                Turnover = 0m,
                WinLoss = 0m,
                AdditionalData = new JsonObject
                {
                    ["id"] = "1776ae69497e716d7b64c0cf"
                }.ToJsonString()
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Void),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Win()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "EVOTF",
            GameProviderConfiguration = "EVOTFConfiguration",
            GameProviderHangfire = "evotf-schedule:hangfire:",
            IntervalMinutes = 10,
            IntervalMinutesMax = 1440,
            IntervalMinutesMinimum = 1
        });
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "1776ae69497e716d7b64c0cf",
                ExternalGameId = "BacBo00000000001",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsCancelled = false,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-30T15:21:17.352Z").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-30T15:21:46.138Z").ToTimestamp(),
                Bet = 25m,
                ValidBet = 25m,
                Settle = 50m,
                Commission = 0m,
                Turnover = 25m,
                WinLoss = 25m,
                AdditionalData = new JsonObject
                {
                    ["id"] = "1776ae69497e716d7b64c0cf"
                }.ToJsonString()
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            status: Arg.Is(GameRecordStatus.Closed),
            clientStatus: Arg.Is(GameRecordClientStatus.Win),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.Turnover.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}

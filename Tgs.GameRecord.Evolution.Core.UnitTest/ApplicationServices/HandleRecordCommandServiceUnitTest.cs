using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Evolution.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Cancelled()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "EVOTF",
			GameProviderConfiguration = "EVOTFConfiguration",
			HangfirePrefix = "gamerecord-evotf-hangfire:",
			HangfireLocking = "gamerecord-evotf-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 60,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Cancel,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 0.0M,
				ValidBet = 0.0M,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "EVOTF",
			GameProviderConfiguration = "EVOTFConfiguration",
			HangfirePrefix = "gamerecord-evotf-hangfire:",
			HangfireLocking = "gamerecord-evotf-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 60,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 25m,
				ValidBet = 25m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 25m,
				WinLoss = -25m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Void()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "EVOTF",
			GameProviderConfiguration = "EVOTFConfiguration",
			HangfirePrefix = "gamerecord-evotf-hangfire:",
			HangfireLocking = "gamerecord-evotf-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 60,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Void,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 0.0M,
				ValidBet = 0.0M,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "EVOTF",
			GameProviderConfiguration = "EVOTFConfiguration",
			HangfirePrefix = "gamerecord-evotf-hangfire:",
			HangfireLocking = "gamerecord-evotf-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 60,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

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

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Record.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 25m,
				ValidBet = 25m,
				Settle = 50m,
				Commission = 0.0m,
				Turnover = 25m,
				WinLoss = 25m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}

using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.EvolutionService.Protos;
using Tgs.GameRecord.Evolution.Core;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.EvolutionService.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Cancelled()
    {
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Void),
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
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Loss),
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
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Void),
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
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

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

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Win),
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

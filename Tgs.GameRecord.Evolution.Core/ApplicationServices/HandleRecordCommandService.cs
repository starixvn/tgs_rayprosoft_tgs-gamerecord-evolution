using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Core.ApplicationServices;

internal class HandleRecordCommandService(
	IOptions<GameRecordInfoOptions> options,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
		var externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
		var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.Record.ValidBet.ToDecimal().GetValueOrDefault(0);
		var settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
		var commission = command.Record.Commission.ToDecimal().GetValueOrDefault(0);
		var turnover = command.Record.Turnover.ToDecimal().GetValueOrDefault(0);
		var winLoss = command.Record.WinLoss.ToDecimal().GetValueOrDefault(0);
		var wagerState =
			command.Record.IsVoid
			? WagerStates.Void
			: command.Record.IsCancelled
			? WagerStates.Refund
			: winLoss > 0
			? WagerStates.Win
			: WagerStates.Lose;

		return client
			.MessageAsync(
			gameProviderCode: _gameRecordInfo.GameProviderCode,
			playerCode: command.Record.PlayerCode,
			wagerState: wagerState,
			wagerType: WagerTypes.Normal,
			freeGameState: FreeGameStates.None,
			gambleState: GambleStates.None,
			cashoutState: CashoutStates.None,
			resettleState: ResettleStates.None,
			externalRoundId: command.Record.ExternalRoundId,
			externalWagerId: command.Record.ExternalRoundId,
			externalGameId: command.Record.ExternalGameId,
			externalCreatedOn: externalCreatedOn,
			externalClosedOn: externalClosedOn,
			externalPayoffOn: externalClosedOn,
			externalResettleOn: DateTimeOffset.UnixEpoch,
			bet: bet,
			validBet: validBet,
			settle: settle,
			commission: commission,
			turnover: turnover,
			winLoss: winLoss,
			additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
			cancellationToken: cancellationToken);
	}
}

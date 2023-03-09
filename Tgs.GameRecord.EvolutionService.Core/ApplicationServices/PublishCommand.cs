namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record PublishCommand(
    string PlayerCode,
    string ExternalRoundId,
    string ExternalGameId,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal SettleAmount,
    string AdditionalData);

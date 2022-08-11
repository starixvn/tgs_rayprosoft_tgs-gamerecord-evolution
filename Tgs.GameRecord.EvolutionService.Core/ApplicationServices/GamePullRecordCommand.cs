using Tgs.GameInterface.EvolutionService.Protos;

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record GamePullRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameHistoryRecord> Records);

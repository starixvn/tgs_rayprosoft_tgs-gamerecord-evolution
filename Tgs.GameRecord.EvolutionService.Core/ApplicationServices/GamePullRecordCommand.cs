using Tgs.GameInterface.EvolutionService.Protos;

namespace Tgs.GameRecord.EvolutionService.Core.ApplicationServices;

public record GamePullRecordCommand(IEnumerable<GameHistoryRecord> Records);

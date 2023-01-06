using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.EvolutionManualService.ViewModels;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionManualService.Controllers;

[ApiController]
[Route("evotf")]
public class EvolutionTransferController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<EvolutionWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new EvolutionWorkerCommand(
            GameApiUrl: model.GameApiUrl,
            CasinoId: model.CasinoId,
            GameHistoryApiToken: model.GameHistoryApiToken,
            StartDate: model.StartDate,
            EndDate: model.EndDate));
}

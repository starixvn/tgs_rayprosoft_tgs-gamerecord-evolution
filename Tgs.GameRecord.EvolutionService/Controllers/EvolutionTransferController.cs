using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.GameRecord.JiliService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Controllers
{
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
                Authorization: model.Authorization,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}

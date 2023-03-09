using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.EvolutionService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Controllers;

[ApiController]
[Route("evotf")]
public class EvolutionTransferController : ControllerBase
{
    /// <summary>
    /// Executes the asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "apiUrl":"https://babylonpega2.uat1.evo-test.com/",
    ///             "casinoId":"babylonpega20001",
    ///             "gameHistoryApiToken":"test123",
    ///             "startTime": "2022-09-07T18:50:00+08:00",
    ///             "endTime": "2022-09-07T18:55:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<GameHistoryCommand> service,
        GameHistoryCommand source)
        => service
        .ExecuteAsync(
            command: source,
            cancellationToken: HttpContext.RequestAborted);

    /// <summary>
    /// Manuals the execute asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "apiUrl":"https://babylonpega2.uat1.evo-test.com/",
    ///             "casinoId":"babylonpega20001",
    ///             "gameHistoryApiToken":"test123",
    ///             "startTime": "2022-09-07T18:50:00+08:00",
    ///             "endTime": "2022-09-07T18:55:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("manual")]
    public ValueTask ManualExecuteAsync(
        [FromServices] ICommandService<GameHistoryManualCommand> service,
        GameHistoryManualCommand source)
        => service
        .ExecuteAsync(
            command: source,
            cancellationToken: HttpContext.RequestAborted);
}

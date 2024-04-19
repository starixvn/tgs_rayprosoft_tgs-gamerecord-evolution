using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Evolution.Common;
using Tgs.GameRecord.Evolution.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Controllers;

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
		[FromServices] ICommandService<PullRecordCommand> service,
		EvolutionGameRecordMessage source)
		=> service
		.ExecuteAsync(
			command: new PullRecordCommand(
				source.ApiUrl,
				source.CasinoId,
				source.GameHistoryApiToken,
				source.StartTime,
				source.EndTime),
			cancellationToken: HttpContext.RequestAborted);
}

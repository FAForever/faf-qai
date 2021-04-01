using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Faforever.Qai.Api
{
	[Route("api/link")]
	[ApiController]
	public class DiscordFafLinkApiController : ControllerBase
	{
		[HttpGet("token/{token}")]
		public async Task<IActionResult> GetTokenEndpointAsync(string token)
		{
			return Ok();
		}

		[HttpGet("login")]
		public async Task<IActionResult> GetLoginEndpointAsync()
		{
			return Ok();
		}

		[HttpGet("auth")]
		public async Task<IActionResult> GetAuthEndpointAsync()
		{
			return Ok();
		}
	}
}

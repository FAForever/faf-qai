using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Faforever.Qai.Api
{
	[Route("api/link")]
	[ApiController]
	public class DiscordFafLinkApiController : ControllerBase
	{
		[HttpGet("token/{token}")]
		public IActionResult GetTokenEndpoint(string token)
		{
			if(User.HasClaim("linked", "true"))
			{
				// do validation stuff

				return Ok();
			}
			else
			{
				Response.Cookies.Append("token", token);
				return Redirect("/api/link/login");
			}
		}

		[HttpGet("login")]
		[Authorize(AuthenticationSchemes = "DISCORD")]
		public IActionResult GetLoginEndpoint()
		{
			return Redirect("/api/link/auth");
		}

		[HttpGet("auth")]
		[Authorize(AuthenticationSchemes = "FAF")]
		public IActionResult GetAuthEndpoint()
		{
			if(Request.Cookies.TryGetValue("token", out var token))
			{
				return Redirect($"/api/link/token/{token}");
			}

			return BadRequest("No token found.");
		}

		[HttpGet("denied")]
		public IActionResult GetDeniedEndpoint()
		{
			return BadRequest("User denied account linking.");
		}
	}
}

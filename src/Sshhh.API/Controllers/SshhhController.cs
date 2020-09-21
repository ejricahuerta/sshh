using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sshhh.API.Services;
using Sshhh.Discord.Model;

namespace Sshhh.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SshhhController : ControllerBase
    {

        private readonly ILogger<SshhhController> _logger;
        private readonly DiscordService discordService;

        public SshhhController(ILogger<SshhhController> logger, DiscordService discordService)
        {
            _logger = logger;
            this.discordService = discordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                await discordService.MainAsync();
                this._logger.LogInformation("Discord Sshhh Server Started");

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);

            }
            return Ok();
        }

        [HttpPost("MuteUsers")]
        public async Task<IActionResult> MuteUsersAsync([FromBody] DiscordInput Input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.ToggleMuteForAllUsersAsync(Input.From, Input.Mute);
                    this._logger.LogInformation($"Discord Sshhh {(Input.Mute ? "Muted" : "UnMuted")} Users");

                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("MoveUsers")]
        public async Task<IActionResult> MoveUsersAsync([FromBody] DiscordInput Input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.MoveUsers(Input.From, Input.To);
                    this._logger.LogInformation("Discord Sshhh Moved Users");

                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                return Ok();
            }
            return BadRequest();

        }
        [HttpPost("SwapUsers")]
        public async Task<IActionResult> SwapUsersAsync([FromBody] DiscordInput Input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.MoveUsers( Input.To, Input.From);
                    this._logger.LogInformation("Discord Sshhh Swapped Users");

                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                return Ok();
            }
            return BadRequest();

        }



        [HttpPost("ClearLogs")]
        public async Task<IActionResult> ClearLogsAsync([FromBody] DiscordInput Input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.FlushChannelLogs();
                    this._logger.LogInformation("Discord Sshhh Cleared Logs");

                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                return Ok();
            }
            return BadRequest();

        }
    }
}


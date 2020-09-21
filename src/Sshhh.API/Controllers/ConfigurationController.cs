using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sshhh.API.Services;
using Sshhh.Discord;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sshhh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly DiscordService discordService;

        public ConfigurationController(DiscordService discordService)
        {
            this.discordService = discordService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return  Ok(discordService.Configuration?? new DiscordConfiguration());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DiscordConfiguration discordConfiguration)
        {
            try
            {
                await discordService.Configure(discordConfiguration);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}

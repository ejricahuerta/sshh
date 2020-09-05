using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sshhh.Web.Discord;
using Sshhh.Web.ViewModel;

namespace Sshhh.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DiscordService discordService;

        [BindProperty]
        public DiscordInput Input { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DiscordService discordService)
        {
            _logger = logger;
            this.discordService = discordService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                await discordService.MainAsync(null);
            }
            catch (Exception e)
            {
                this._logger.LogInformation(e, "Unable to Process");
            }
        }

        public async Task OnPostMuteUsersAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.ToggleMuteForAllUsersAsync(Input.ServerId, Input.From, true);
                }
                catch (Exception e)
                {
                    this._logger.LogInformation(e, "Unable to Process");
                }
            }
        }

        public async Task OnPostUnMuteUsersAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await discordService.ToggleMuteForAllUsersAsync(Input.ServerId, Input.From, false);
                }
                catch (Exception e)
                {
                    this._logger.LogInformation(e, "Unable to Process");
                }
            }
        }


        public async Task OnPostMoveAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {

                    await discordService.MoveUsers(Input.ServerId, Input.From, Input.To);
                }
                catch (Exception e)
                {
                    this._logger.LogInformation(e, "Unable to Process");
                }
            }
        }

        public async Task OnPostMoveSwapAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {

                    await discordService.MoveUsers(Input.ServerId, Input.To, Input.From);
                }
                catch (Exception e)
                {
                    this._logger.LogInformation(e, "Unable to Process");
                }
            }
        }
    }
}

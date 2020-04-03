using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Financial.Shared.Handling;
using System;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Web.Controllers
{
    [ApiController]
    [Route("api/financial-entry")]
    public class FinancialEntryController : ControllerBase
    {
        private readonly CommandProcessor commands;
        private readonly RequestProcessor requests;

        /// <summary>Initializes a new instance of the <see cref="FinancialEntryController"/> class.</summary>
        public FinancialEntryController(CommandProcessor commands, RequestProcessor requests)
        {
            this.commands = Guard.NotNull(commands, nameof(commands));
            this.requests = Guard.NotNull(requests, nameof(requests));
        }

        /// <summary>Creates a finacial entry.</summary>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<IActionResult> CreateAsync(CreateFinancialEntry cmd)
        {
            Guard.NotNull(cmd, nameof(cmd));
            cmd.Id = Guid.NewGuid();
            return ApiAction.PostAsync(commands.SendAsync(cmd), $"api/financial-entry/{cmd.Id}");
        }

        /// <summary>Gets the specified financial entyr for the given ID.</summary>
        [HttpGet("{entryId}")]
        [Produces(typeof(ApiResponse<object>))]
        public Task<IActionResult> GetAsync(Guid entryId)
        {
            return ApiAction.GetAsync(requests.SendAsync<Guid, object>(entryId));
        }
    }
}

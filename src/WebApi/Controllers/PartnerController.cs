using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Application.Partners.CreatePartner;
using MyWarehouse.Application.Partners.DeletePartner;
using MyWarehouse.Application.Partners.GetPartnerDetails;
using MyWarehouse.Application.Partners.GetPartners;
using MyWarehouse.Application.Partners.UpdatePartner;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("partners")]
    public class PartnerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PartnerController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreatePartnerCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<ActionResult<IListResponseModel<PartnerDto>>> GetList([FromQuery] ListQueryModel<PartnerDto> query)
            => Ok(await _mediator.Send(query));

        [HttpGet("{id}")]
        public async Task<ActionResult<PartnerDetailsDto>> Get(int id)
            => Ok(await _mediator.Send(new GetPartnerDetailsQuery() { Id = id }));

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeletePartnerCommand() { Id = id });

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdatePartnerCommand command)
        {
            if (id != command.Id) return BadRequest();

            await _mediator.Send(command);

            return NoContent();
        }
    }
}

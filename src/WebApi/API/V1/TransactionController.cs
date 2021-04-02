using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Application.Transactions.CreateTransaction;
using MyWarehouse.Application.Transactions.GetTransactionDetails;
using MyWarehouse.Application.Transactions.GetTransactionsList;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.API.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateTransactionCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<ActionResult<IListResponseModel<TransactionDto>>> GetList([FromQuery] GetTransactionListQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDetailsDto>> Get(int id)
            => Ok(await _mediator.Send(new GetTransactionDetailsQuery() { Id = id }));
    }
}

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Common.Behaviors
{
    public class ErrorHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public ErrorHandlerBehavior()
        {

        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception e)
            {
                throw;
                // TODO: Implement logging as an application (instead of API).
            }
        }
    }
}
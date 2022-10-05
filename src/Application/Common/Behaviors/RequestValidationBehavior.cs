using FluentValidation;
using MediatR;
using MyWarehouse.Application.Common.Exceptions;
namespace MyWarehouse.Application.Common.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(_validators
                .Select(v => v.ValidateAsync(context)));

            var failures = results
                .Where(r => !r.IsValid)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                throw new InputValidationException(failures);
            }
        }

        return await next();
    }
}

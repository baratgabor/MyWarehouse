using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Infrastructure.Authentication.External.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyWarehouse.WebApi.ErrorHandling
{
    /// <summary>
    /// Maps exceptions occurred in lower layers into HTTP responses with appropriate HTTP code.
    /// Make sure to register this filter globally.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        { 
            //TODO: Push logging deeper, into Application layer, and dedicate this component to response mapping.
            _logger.LogError($"Exception: [{context.Exception.Message}]\r\n\r\nStack Trace:\r\n{context.Exception.StackTrace}");
            context.Result = ExecuteHandler(context.Exception);
            context.ExceptionHandled = true;

            base.OnException(context);
        }

        [ExcludeFromCodeCoverage] // Seems like coverlet doesn't register switch expressions properly; it is tested.
        private static IActionResult ExecuteHandler(Exception exception)
            => exception switch
            {
                InputValidationException e => HandleValidationException(e),
                EntityNotFoundException e => HandleNotFoundException(e),
                ExternalAuthenticationPreventedException e => HandleCannotAuthenticateExternal(e),
                _ => HandleUnknownException(exception)
            };

        private static IActionResult HandleCannotAuthenticateExternal(ExternalAuthenticationPreventedException e)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "External authentication was prevented. Authentication provider might be unavailable. Try again later.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            };

            return new ObjectResult(details) { StatusCode = StatusCodes.Status503ServiceUnavailable };
        }

        private static IActionResult HandleUnknownException(Exception _)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            return new ObjectResult(details) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        private static IActionResult HandleValidationException(InputValidationException exception)
        {
            var details = new ValidationProblemDetails(exception.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            return new BadRequestObjectResult(details);
        }

        private static IActionResult HandleNotFoundException(EntityNotFoundException exception)
        {
            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception.Message
            };

            return new NotFoundObjectResult(details);
        }
    }
}

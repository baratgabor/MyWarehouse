using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.WebApi.ErrorHandling;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MyWarehouse.WebApi.UnitTests.ErrorHandling
{
    public class ApiExceptionFilterTests
    {
        private Mock<ILogger<ApiExceptionFilter>> _mockLogger;
        private ApiExceptionFilter _sut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<ApiExceptionFilter>>();
            _sut = new ApiExceptionFilter(_mockLogger.Object);
        }

        private ExceptionContext GetExceptionContext(Exception exception)
        {
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }

        [Test]
        public void OnException_WithEntityNotFoundException_SetsNotFoundObjectResult()
        {
            var ctx = GetExceptionContext(new EntityNotFoundException());
            
            _sut.OnException(ctx);

            ctx.Result.Should().BeAssignableTo(typeof(NotFoundObjectResult));
        }

        [Test]
        public void OnException_WithInvalidValidationException_SetsBadRequestResponse()
        {
            var ctx = GetExceptionContext(new InputValidationException());
            
            _sut.OnException(ctx);

            ctx.Result.Should().BeAssignableTo(typeof(BadRequestObjectResult));
        }

        [Test]
        public void OnException_WithUnknownException_SetsInternalServerErrorResponse()
        {
            var ctx = GetExceptionContext(new Exception());

            _sut.OnException(ctx);

            ctx.Result.Should().BeAssignableTo(typeof(ObjectResult));
            ((ObjectResult)ctx.Result).StatusCode.Should().Be(500);
        }
    }
}

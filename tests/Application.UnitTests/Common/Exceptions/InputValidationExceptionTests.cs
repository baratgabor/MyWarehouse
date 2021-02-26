using FluentAssertions;
using FluentValidation.Results;
using MyWarehouse.Application.Common.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWarehouse.Application.UnitTests.Common.Exceptions
{
    public class InputValidationExceptionTests
    {

        [Test]
        public void Instantiation_Blank_SetsProperties()
        {
            var innerException = new Exception();
            
            var sut = new InputValidationException(innerException);

            sut.InnerException.Should().Be(innerException);
            sut.Message.Should().Contain("validation");
            sut.Errors.Should().NotBeNull();
        }

        [Test]
        public void Instantiation_WithValidationFailures_SetsProperties()
        {
            var failures = new List<ValidationFailure>()
            {
                new ValidationFailure("Prop", "Prop not good, mister."),
                new ValidationFailure("Prop", "Prop really not good, mister.")
            };

            var sut = new InputValidationException(failures);

            sut.InnerException.Should().BeNull();
            sut.Message.Should().Contain("validation");
            sut.Errors.Should().HaveCount(1).And.ContainKey("Prop");
            sut.Errors.First().Should().BeEquivalentTo(
                new {
                    Key = "Prop",
                    Value = new[] {
                        failures[0].ErrorMessage,
                        failures[1].ErrorMessage
                    }
                },
                because: "Validations failured are expected to be grouped by property name."
            );
        }

        [Test]
        public void Instantiation_WithTuples_SetsProperties()
        {
            var failureTuples = new[]
            {
                ("Prop", "Prop not good, mister."),
                ("Prop", "Prop really not good, mister.")
            };

            var sut = new InputValidationException(failureTuples);

            sut.InnerException.Should().BeNull();
            sut.Message.Should().Contain("validation");
            sut.Errors.Should().HaveCount(1).And.ContainKey("Prop");
            sut.Errors.First().Should().BeEquivalentTo(
                new
                {
                    Key = "Prop",
                    Value = new[] {
                        failureTuples[0].Item2,
                        failureTuples[1].Item2
                    }
                },
                because: "Validations failured are expected to be grouped by property name."
            );
        }
    }
}

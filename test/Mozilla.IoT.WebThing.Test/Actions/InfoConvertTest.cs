using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Actions;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Actions
{
    public class ActionInfoConvertTest
    {
        private readonly Fixture _fixture;
        private readonly Dictionary<string, IActionParameter> _parameters;
        private readonly DictionaryInputConvert _convert;

        public ActionInfoConvertTest()
        {
            _fixture = new Fixture();
            _parameters = new Dictionary<string, IActionParameter>(StringComparer.InvariantCultureIgnoreCase);
            _convert = new DictionaryInputConvert(_parameters);
        }

        [Fact]
        public void TryConvertWithSuccess()
        {
            var parameter = Substitute.For<IActionParameter>();

            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            _parameters.Add(parameterName, parameter);
            
            parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>() )
                .Returns(x =>
                {
                    x[1] = parameterValue;
                    return true;
                });
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{parameterName}"": {parameterValue} }}");
            _convert.TryConvert(json, out var result).Should().BeTrue();

            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                [parameterName] = parameterValue
            });

            parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>());
        }
        
        [Fact]
        public void TryConvertWithSuccessWhenActionCanBeNull()
        {
            var parameter = Substitute.For<IActionParameter>();

            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            _parameters.Add(parameterName, parameter);
            
            parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>() )
                .Returns(x =>
                {
                    x[1] = parameterValue;
                    return true;
                });

            var parameterNullName = _fixture.Create<string>();
            var parameterNull = Substitute.For<IActionParameter>();
            parameterNull.CanBeNull.Returns(true);
            _parameters.Add(parameterNullName, parameterNull);
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                ""{parameterName}"": {parameterValue} 
            }}");
            _convert.TryConvert(json, out var result).Should().BeTrue();

            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new Dictionary<string, object?>
            {
                [parameterName] = parameterValue,
                [parameterNullName] = null
            });
            
            parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>());
            
            _ = parameterNull
                .Received(1)
                .CanBeNull;
        }
        
        [Fact]
        public void TryConvertWhenParameterDoesNotExist()
        {
            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{parameterName}"": {parameterValue} }}");
            _convert.TryConvert(json, out var result).Should().BeFalse();

            result.Should().BeNull();
        }
        
        [Fact]
        public void TryConvertWhenCouldNotGet()
        {
            var parameter = Substitute.For<IActionParameter>();

            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            _parameters.Add(parameterName, parameter);
            
            parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>() )
                .Returns(x =>
                {
                    x[1] = null;
                    return false;
                });
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ ""{parameterName}"": {parameterValue} }}");
            _convert.TryConvert(json, out var result).Should().BeFalse();

            result.Should().BeNull();
            
            parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>());
        }
        
        [Fact]
        public void TryConvertWhenInputAlreadyExist()
        {
            var parameter = Substitute.For<IActionParameter>();

            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            _parameters.Add(parameterName, parameter);
            
            parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>() )
                .Returns(x =>
                {
                    x[1] = parameterValue;
                    return true;
                });
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                ""{parameterName}"": {parameterValue},
                ""{parameterName}"": {parameterValue} 
            }}");
            
            _convert.TryConvert(json, out var result).Should().BeFalse();

            result.Should().BeNull();
            
            parameter
                .Received(2)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>());
        }
        
        [Fact]
        public void TryConvertWhenActionCannotBeNull()
        {
            var parameter = Substitute.For<IActionParameter>();

            var parameterName = _fixture.Create<string>();
            var parameterValue = _fixture.Create<int>();
            _parameters.Add(parameterName, parameter);
            
            parameter.TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>() )
                .Returns(x =>
                {
                    x[1] = parameterValue;
                    return true;
                });

            var parameterNullName = _fixture.Create<string>();
            var parameterNull = Substitute.For<IActionParameter>();
            
            parameterNull.CanBeNull.Returns(false);
            _parameters.Add(parameterNullName, parameterNull);
            
            var json = JsonSerializer.Deserialize<JsonElement>($@"{{ 
                ""{parameterName}"": {parameterValue} 
            }}");
            _convert.TryConvert(json, out var result).Should().BeFalse();

            result.Should().BeNull();

            parameter
                .Received(1)
                .TryGetValue(Arg.Any<JsonElement>(), out Arg.Any<object?>());
            
            _ = parameterNull
                .Received(1)
                .CanBeNull;
        }
    }
}

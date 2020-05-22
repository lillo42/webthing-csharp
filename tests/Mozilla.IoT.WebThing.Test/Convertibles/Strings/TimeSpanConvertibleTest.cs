using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class TimeSpanConvertibleTest : BaseConvertibleTest<TimeSpan>
    {
        protected override IConvertible CreateConvertible()
        {
            return TimeSpanConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<TimeSpan>();
            convertible.Convert(value.ToString())
                .Should().Be(value);
        }
    }
}

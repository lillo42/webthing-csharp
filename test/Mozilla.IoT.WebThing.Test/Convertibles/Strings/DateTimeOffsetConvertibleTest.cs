using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class DateTimeOffsetConvertibleTest : BaseConvertibleTest<DateTimeOffset>
    {
        protected override IConvertible CreateConvertible()
        {
            return DateTimeOffsetConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<DateTimeOffset>();
            convertible.Convert(value.ToString("O"))
                .Should().Be(DateTimeOffset.Parse(value.ToString("O")));
        }
    }
}

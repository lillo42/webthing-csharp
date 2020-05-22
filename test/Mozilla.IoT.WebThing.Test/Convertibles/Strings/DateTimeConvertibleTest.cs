using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class DateTimeConvertibleTest : BaseConvertibleTest<DateTime>
    {
        protected override IConvertible CreateConvertible()
        {
            return DateTimeConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<DateTime>();
            convertible.Convert(value.ToString("O"))
                .Should().Be(DateTime.Parse(value.ToString("O")));
        }
    }
}

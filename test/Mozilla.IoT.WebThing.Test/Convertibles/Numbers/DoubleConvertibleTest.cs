using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Number;
using Mozilla.IoT.WebThing.Factories;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Numbers
{
    public class DoubleConvertibleTest : BaseConvertibleTest<double>
    {
        protected override IConvertible CreateConvertible()
        {
            return new NumberConvertible(TypeCode.Double);
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<double>();
            convertible.Convert(value.ToString(CultureInfo.InvariantCulture))
                .Should().Be(value);
        }
    }
}

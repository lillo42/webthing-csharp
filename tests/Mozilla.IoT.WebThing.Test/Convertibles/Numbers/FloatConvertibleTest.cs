using System.Globalization;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Number;
using Mozilla.IoT.WebThing.Factories;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Numbers
{
    public class FloatConvertibleTest : BaseConvertibleTest<float>
    {
        protected override IConvertible CreateConvertible()
        {
            return new NumberConvertible(TypeCode.Float);
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<float>();
            convertible.Convert(value.ToString(CultureInfo.InvariantCulture))
                .Should().Be(value);
        }
    }
}

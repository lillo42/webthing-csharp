using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Boolean
{
    public class BooleanConvertibleTest : BaseConvertibleTest<bool>
    {
        protected override IConvertible CreateConvertible()
        {
            return BooleanConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<bool>();
            convertible.Convert(value.ToString())
                .Should().Be(value);
        }
    }
}

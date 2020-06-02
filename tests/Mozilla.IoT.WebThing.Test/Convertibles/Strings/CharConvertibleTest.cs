using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class CharConvertibleTest : BaseConvertibleTest<char>
    {
        protected override IConvertible CreateConvertible()
        {
            return CharConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<char>();
            convertible.Convert(value.ToString())
                .Should().Be(value);
        }
    }
}

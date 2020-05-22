using System;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles.Strings;
using Xunit;
using IConvertible = Mozilla.IoT.WebThing.Convertibles.IConvertible;

namespace Mozilla.IoT.WebThing.Test.Convertibles.Strings
{
    public class GuidConvertibleTest : BaseConvertibleTest<Guid>
    {
        protected override IConvertible CreateConvertible()
        {
            return GuidConvertible.Instance;
        }

        [Fact]
        public void Create_Should_ConvertType_When_ValueIsOtherType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<Guid>();
            convertible.Convert(value.ToString())
                .Should().Be(value);
        }
    }
}

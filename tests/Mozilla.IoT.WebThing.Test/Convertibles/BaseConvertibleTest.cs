using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Convertibles;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Convertibles
{
    public abstract class BaseConvertibleTest<T>
    {
        protected Fixture Fixture { get; }

        public BaseConvertibleTest()
        {
            Fixture = new Fixture();
        }

        protected abstract IConvertible CreateConvertible();

        [Fact]
        public void Convert_Should_ReturnNull_When_ValueIsNull()
        {
            var convertible = CreateConvertible();
            convertible.Convert(null).Should().Be(null);
        }
        
        [Fact]
        public void Convert_Should_ReturnValue_When_ValueIsSameType()
        {
            var convertible = CreateConvertible();
            var value = Fixture.Create<T>();
            convertible.Convert(value).Should().Be(value);
        }
    }
}

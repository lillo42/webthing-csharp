using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Mozilla.IoT.WebThing.Test
{
    public class PropertyTest
    {
        private readonly Fixture _fixture;

        public PropertyTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task OnPropertyChanged_Should_Emit_When_ValueChange()
        {
            var property = new Property();

            var newValue = _fixture.Create<string>();
            var oldValue = property.Value;
            var change = false;


            property.ValuedChanged += (sender, @event) =>
            {
                sender.Should().Be(property);
                @event.Value.Should().NotBe(oldValue);
                @event.Value.Should().Be(newValue);
                ((Property)sender).Value.Should().Be(@event.Value);
                change = true;
            };

            property.Value = newValue;

            while (!change)
            {
                await Task.Delay(1);
            }
        }


        [Fact]
        public void Href_Should_BeDefault()
        {
            var name = _fixture.Create<string>();
            var property = new Property {Name = name};

            property.Href.Should().Be($"/properties/{name}");
        }
        
        [Fact]
        public async Task GenericOnPropertyChanged_Should_Emit_When_ValueChange()
        {
            var property = new Property<int>();

            var newValue = _fixture.Create<int>();
            var oldValue = property.Value;
            var change = false;

            property.ValuedChanged += (sender, @event) =>
            {
                sender.Should().Be(property);
                @event.Value.Should().NotBe(oldValue);
                @event.Value.Should().Be(newValue);
                ((Property<int>)sender).Value.Should().Be(@event.Value);
                change = true;
            };

            property.Value = newValue;

            while (!change)
            {
                await Task.Delay(1);
            }
        }


        [Fact]
        public void GenericHref_Should_BeDefault()
        {
            var name = _fixture.Create<string>();
            var property = new Property<int> {Name = name};

            property.Href.Should().Be($"/properties/{name}");
        }
    }
}

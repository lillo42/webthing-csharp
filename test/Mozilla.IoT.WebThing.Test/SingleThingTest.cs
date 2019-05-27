using System.Linq;
using AutoFixture;
using Xunit;

using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test
{
    public class SingleThingTest
    {
        private readonly Thing _thing;
        private readonly SingleThing _single;
        private readonly Fixture _fixture;

        public SingleThingTest()
        {
            _fixture = new Fixture();
            _thing = _fixture.Create<Thing>();
            _single = new SingleThing(_thing);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Index(int id)
        {
            NotNull(_single[id]);
            Equal(_thing, _single[id]);
        }
        
        [Fact]
        public void Name()
        {
            Equal(_thing.Name, _single.Name);
        }
        
        [Fact]
        public void Things()
        {
            NotNull(_single.Things);
            NotEmpty(_single.Things);
            True(_single.Things.Count() == 1);
            Equal(_thing, _single.Things.First());
        }
    }
}

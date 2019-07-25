//using System.Collections.Generic;
//using System.Linq;
//using AutoFixture;
//using Xunit;
//
//using static Xunit.Assert;
//
//namespace Mozilla.IoT.WebThing.Test
//{
//    public class MultipleThingsTest
//    {
//        private readonly IList<Thing > _things ;
//        private readonly MultipleThings _multi;
//        private readonly Fixture _fixture;
//        private readonly string _name;
//        
//        public MultipleThingsTest()
//        {
//            _fixture = new Fixture();
//            _things = new List<Thing>
//            {
//                _fixture.Create<Thing>(),
//                _fixture.Create<Thing>()
//            };
//                
//            _name = _fixture.Create<string>();
//            _multi = new MultipleThings(_things, _name);
//        }
//        
//        [Theory]
//        [InlineData(1)]
//        [InlineData(0)]
//        public void Index_Not_Null(int id)
//        {
//            NotNull(_multi[id]);
//            Equal(_things[id], _multi[id]);
//        }
//        
//        [Theory]
//        [InlineData(-1)]
//        [InlineData(2)]
//        public void Index_Null(int id)
//        {
//            Null(_multi[id]);
//        }
//        
//        [Fact]
//        public void Name()
//        {
//            Equal(_multi.Name,_name);
//        }
//        
//        [Fact]
//        public void Things()
//        {
//            NotNull(_multi.Things);
//            NotEmpty(_multi.Things);
//            True(_multi.Things.Count() == _things.Count);
//        }
//    }
//}

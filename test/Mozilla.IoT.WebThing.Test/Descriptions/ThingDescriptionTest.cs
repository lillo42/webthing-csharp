using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Descriptor;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Descriptions
{
    public class ThingDescriptionTest
    {
        private readonly Fixture _fixture;
        private readonly Thing _thing;
        private readonly ThingDescriptor _descriptor;

        public ThingDescriptionTest()
        {
            _fixture = new Fixture();
            _thing = new Thing();
            _descriptor = new ThingDescriptor(new PropertyDescriptor());
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] = new Dictionary<string, object>(),
                ["events"] = new Dictionary<string, object>(),
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveDescriptionAndUiHref()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();
            string description = _fixture.Create<string>();
            string ui = _fixture.Create<string>();

            _thing.Name = name;
            _thing.Type = types;
            _thing.Description = description;
            _thing.UiHref = ui;

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["description"] = description,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] = new Dictionary<string, object>(),
                ["events"] = new Dictionary<string, object>(),
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"},
                    new Dictionary<string, object> {["rel"] = "alternate", ["href"] = ui, ["mediaType"] = "text/html"},
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveAction()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            _thing.AddAction<CustomAction>();

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "action", ["href"] = "/actions/custom"
                                }
                            }
                        }
                    },
                ["events"] = new Dictionary<string, object>(),
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveActionWithMetadata()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();


            _thing.AddAction<CustomAction>(new Dictionary<string, object> {[key] = value});

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            [key] = value,
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "action", ["href"] = "/actions/custom"
                                }
                            }
                        }
                    },
                ["events"] = new Dictionary<string, object>(),
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveEvent()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            _thing.AddAction<CustomAction>();
            _thing.AddEvent<CustomEvent>();

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "action", ["href"] = "/actions/custom"
                                }
                            }
                        }
                    },
                ["events"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "event", ["href"] = "/events/custom"
                                }
                            }
                        }
                    },
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveEventWithMetadata()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();


            _thing.AddAction<CustomAction>(new Dictionary<string, object> {[key] = value});

            _thing.AddEvent<CustomEvent>(new Dictionary<string, object> {[key] = value});

            var result = _descriptor.CreateDescription(_thing);

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            [key] = value,
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "action", ["href"] = "/actions/custom"
                                }
                            }
                        }
                    },
                ["events"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            [key] = value,
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "event", ["href"] = "/events/custom"
                                }
                            }
                        }
                    },
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        [Fact]
        public void CreateDescription_Should_ReturnDescription_When_HaveProperty()
        {
            string name = _fixture.Create<string>();
            var types = _fixture.Create<List<string>>();

            _thing.Name = name;
            _thing.Type = types;

            _thing.AddAction<CustomAction>();
            _thing.AddEvent<CustomEvent>();
            var property = new Property {Name = _fixture.Create<string>(),};


            _thing.Properties.Add(property);

            var result = _descriptor.CreateDescription(_thing);

            var propertyDescription =new Dictionary<string, object>()
                {
                    [property.Name] = new PropertyDescriptor().CreateDescription(property)
                };

            result.Should().BeEquivalentTo(new Dictionary<string, object>
            {
                ["name"] = name,
                ["href"] = "/",
                ["@context"] = "https://iot.mozilla.org/schemas",
                ["@type"] = types,
                ["properties"] = new Dictionary<string, object>(),
                ["actions"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "action", ["href"] = "/actions/custom"
                                }
                            }
                        }
                    },
                ["properties"] = propertyDescription,
                ["events"] =
                    new Dictionary<string, object>
                    {
                        ["custom"] = new Dictionary<string, object>
                        {
                            ["links"] = new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    ["rel"] = "event", ["href"] = "/events/custom"
                                }
                            }
                        }
                    },
                ["links"] = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object> {["rel"] = "properties", ["href"] = "/properties"},
                    new Dictionary<string, object> {["rel"] = "actions", ["href"] = "/actions"},
                    new Dictionary<string, object> {["rel"] = "events", ["href"] = "/events"}
                }
            });
        }

        private class CustomAction : Action
        {
            protected override ValueTask ExecuteAsync(CancellationToken cancellation)
            {
                throw new System.NotImplementedException();
            }
        }

        private class CustomEvent : Event
        {
        }
    }
}

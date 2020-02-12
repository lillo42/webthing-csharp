using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Mozilla.IoT.WebThing.Endpoints;

namespace Mozilla.IoT.WebThing
{
    internal sealed class ServiceRouteBuilder
    {
        private readonly ThingBindingOption _option;
        private bool _isSet = false;

        public ServiceRouteBuilder(ThingBindingOption option)
        {
            _option = option;
        }

        internal IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var result = new LinkedList<IEndpointConventionBuilder>();

            if (!_isSet)
            {
                string prefix = string.Empty;
                
                #region Thing
                
                if (!_option.IsSingleThing)
                {
                    prefix = "/things/{thing}";
                    result.AddLast(endpointRouteBuilder.MapGet("/", GetThings.Invoke));
                    result.AddLast(endpointRouteBuilder.MapGet(prefix, GetThing.Invoke));
                    
                }
                else
                {
                    result.AddLast(endpointRouteBuilder.MapGet("/", GetThing.Invoke));
                }
                
                #endregion
                
                #region Actions

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/actions",
                    GetActions.Invoke));

                result.AddLast(endpointRouteBuilder.MapPost($"{prefix}/actions",
                    PostActions.Invoke));

                
                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/actions/{{name}}",
                    GetAction.Invoke));

                result.AddLast(endpointRouteBuilder.MapPost($"{prefix}/actions/{{name}}",
                    PostAction.Invoke));
                

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/actions/{{name}}/{{id}}",
                    GetActionById.Invoke));

                result.AddLast(endpointRouteBuilder.MapDelete($"{prefix}/actions/{{name}}/{{id}}",
                    DeleteActionById.Invoke));

                #endregion

                #region Events

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/events", GetEvents.Invoke));

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/events/{{name}}", GetEvent.Invoke));

                #endregion

                #region Property

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/properties", GetProperties.Invoke));

                result.AddLast(endpointRouteBuilder.MapGet($"{prefix}/properties/{{name}}", GetProperty.Invoke));

                result.AddLast(endpointRouteBuilder.MapPut($"{prefix}/properties/{{name}}", PutProperty.Invoke));

                #endregion

                _isSet = true;
            }

            return result;
        }
    }
}

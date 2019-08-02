using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Mozilla.IoT.WebThing
{
    internal sealed class ServiceRouteBuilder
    {
        private readonly ThingBindingOption _option;
        private bool _isGenericSet = false;

        public ServiceRouteBuilder(ThingBindingOption option)
        {
            _option = option;
        }

        internal IEnumerable<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var result = new LinkedList<IEndpointConventionBuilder>();

            if (!_isGenericSet)
            {
                endpointRouteBuilder.MapGet("")
                /*
                 *  router.MapMiddlewareGet($"{prefix}/actions/{{actionName}}/{{actionId}}",
                builder => builder.UseMiddleware<GetActionByIdMiddleware>(thingType));

            router.MapMiddlewareDelete($"{prefix}/actions/{{actionName}}/{{actionId}}",
                builder => builder.UseMiddleware<DeleteActionByIdMiddleware>(thingType));


            router.MapMiddlewareGet($"{prefix}/actions/{{actionName}}",
                builder => builder.UseMiddleware<GetActionMiddleware>(thingType));

            router.MapMiddlewarePost($"{prefix}/actions/{{actionName}}",
                builder => builder.UseMiddleware<PostActionMiddleware>(thingType));

            router.MapMiddlewareGet($"{prefix}/actions",
                builder => builder.UseMiddleware<GetActionsMiddleware>(thingType));

            router.MapMiddlewarePost($"{prefix}/actions",
                builder => builder.UseMiddleware<PostActionsMiddleware>(thingType));
                 */
                
                _isGenericSet = true;
            }

            return result;
        }
    }
}

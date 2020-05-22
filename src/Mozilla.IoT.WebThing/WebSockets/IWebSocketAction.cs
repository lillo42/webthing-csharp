using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Web socket action.
    /// </summary>
    public interface IWebSocketAction
    {
        /// <summary>
        /// The Action name. This value should be unique.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// Execute this action when web socket request action where action name match with <see cref="Action"/>
        /// </summary>
        /// <param name="socket">The <see cref="WebSocket"/> origin of this action.</param>
        /// <param name="thing">The <see cref="Thing"/> associated with action.</param>
        /// <param name="data">The <see cref="object"/> request with this action.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> for this action. Every request is generate new scope.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, object data, 
            IServiceProvider provider, CancellationToken cancellationToken);
    }
}

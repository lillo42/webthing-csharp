using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing
{
    public class ThingWebSocket : WebSocket
    {
        private readonly WebSocket _webSocket;
        private readonly Thing _thing;

        public ThingWebSocket(WebSocket webSocket, Thing thing)
        {
            _webSocket = webSocket;
            _thing = thing;
        }

        public override void Abort() 
            => _webSocket.Abort();

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            _thing.RemoveSubscriber(this);
            return _webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) 
            => _webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);

        public override void Dispose() 
            => _webSocket.Dispose();

        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer,
            CancellationToken cancellationToken)
        {
            //TODO: Implement WebSocket
            return _webSocket.ReceiveAsync(buffer, cancellationToken);
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
            CancellationToken cancellationToken) 
            => _webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public override WebSocketCloseStatus? CloseStatus => _webSocket.CloseStatus;
        public override string CloseStatusDescription => _webSocket.CloseStatusDescription;
        public override WebSocketState State => _webSocket.State;
        public override string SubProtocol => _webSocket.SubProtocol;
    }
}

using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SM.Core.Web.Commands;

namespace SM.Core.Web
{
    public class WebSocketMessageMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly WebSocketMessageManager _webSocketMessageManager;

        public WebSocketMessageMiddleware(RequestDelegate requestDelegate,
            WebSocketMessageManager webSocketMessageManager)
        {
            _requestDelegate = requestDelegate;
            _webSocketMessageManager = webSocketMessageManager;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (!httpContext.WebSockets.IsWebSocketRequest)
                return;

            var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

            //добавляем сокет в менеджер
            _webSocketMessageManager.Connect(webSocket);
            //слушаем пока открыт
            await Listen(webSocket);
        }

        private async Task Listen(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        ReceiveMessage(webSocket, buffer, result.Count);
                        break;
                    case WebSocketMessageType.Close:
                        await _webSocketMessageManager.RemoveConnection(webSocket);
                        break;
                }
            }
        }

        private void ReceiveMessage(WebSocket webSocket, byte[] buffer, int bufferSize)
        {

            var json = Encoding.ASCII.GetString(buffer, 0, bufferSize);
            var message = JsonSerializer.Deserialize<WebSocketMessage>(json);

            switch (message.MessageType)
            {
                case nameof(SubscribeCommand):
                    _webSocketMessageManager.SubscribeMessage(webSocket,
                        JsonSerializer.Deserialize<SubscribeCommand>(message.Data));
                    break;
                case nameof(UnsubscribeCommand):
                    _webSocketMessageManager.UnsubscribeMessage(webSocket,
                        JsonSerializer.Deserialize<UnsubscribeCommand>(message.Data));
                    break;
            }

        }
    }
}
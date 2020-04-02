using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SM.Contracts.Commands;

namespace SM.Core.Web
{
    /// <summary>
    /// Менеджер управляющий соединениями вебсокетов
    /// </summary>
    public class WebSocketMessageManager
    {
        private readonly ConcurrentDictionary<WebSocket, List<string>> _connectionSockets =
            new ConcurrentDictionary<WebSocket, List<string>>();

        /// <summary>
        /// Добавление соединения
        /// </summary>
        /// <param name="webSocket"></param>
        public void Connect(WebSocket webSocket)
            => _connectionSockets.TryAdd(webSocket, new List<string>());

        /// <summary>
        /// Удаление соединения
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public Task RemoveConnection(WebSocket webSocket)
        {
            _connectionSockets.TryRemove(webSocket, out var messageTypes);

            return webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed",
                CancellationToken.None);
        }

        /// <summary>
        /// Подписка на оповещение по типу сообщения для сокета
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="command"></param>
        public void SubscribeMessage(WebSocket webSocket, SubscribeCommand command)
        {
            if (!_connectionSockets.TryGetValue(webSocket, out var messageTypes))
                return;

            messageTypes.Add(command.MessageType);
        }

        /// <summary>
        /// Отписка от оповещения по типу сообщения для сокета
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="command"></param>
        public void UnsubscribeMessage(WebSocket webSocket, UnsubscribeCommand command)
        {
            if (!_connectionSockets.TryGetValue(webSocket, out var messageTypes))
                return;

            messageTypes.Remove(command.MessageType);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendMessage<TMessage>(TMessage message) where TMessage : IMessage
        {
            // выбираем все открытие сокеты подписавшиеся на оповещения это типа сообщения
            var sockets = _connectionSockets
                .Where(x => x.Key.State == WebSocketState.Open && x.Value.Contains(typeof(TMessage).Name))
                .ToList();

            if (!sockets.Any())
                return Task.CompletedTask;

            var webMessage = new WebSocketMessage(message);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var messageJson = JsonSerializer.Serialize(webMessage, serializeOptions);

            var byteArray = Encoding.ASCII.GetBytes(messageJson);

            var segment = new ArraySegment<byte>(byteArray, 0, byteArray.Length);

            return Task.WhenAll(sockets.Select(x => x.Key
                .SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None)));
        }
    }
}
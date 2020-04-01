using System.Text.Json;

namespace SM.Core.Web
{
    /// <summary>
    /// Сообщение для общения между сокетами 
    /// </summary>
    public class WebSocketMessage
    {
        // нужен для десериализации
        public WebSocketMessage(){}

        public WebSocketMessage(object data)
        {
            MessageType = data.GetType().Name;
            Data = JsonSerializer.Serialize(data);
        }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Сериализованый объект
        /// </summary>
        public string Data { get; set; }
    }
}
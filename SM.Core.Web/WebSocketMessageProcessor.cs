using System.Threading.Tasks;

namespace SM.Core.Web
{
    /// <summary>
    /// Обработчик сообщений
    /// </summary>
    public class WebSocketMessageProcessor : IMessageProcessor
    {
        private readonly WebSocketMessageManager _messageManager;

        public WebSocketMessageProcessor(WebSocketMessageManager messageManager)
        {
            _messageManager = messageManager;
        }

        /// <summary>
        /// Опубликовать сообщения
        /// </summary>
        /// <typeparam name="TMessage">Тип сообщения</typeparam>
        /// <param name="message">объект сообщения</param>
        public Task Publish<TMessage>(TMessage message) where TMessage : IMessage
            => _messageManager.SendMessage(message);
    }
   
}
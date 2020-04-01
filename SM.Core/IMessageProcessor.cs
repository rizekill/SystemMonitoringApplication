using System.Threading.Tasks;

namespace SM.Core
{
    /// <summary>
    /// Интерфейс обработчика сообщений
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Опубликовать сообщения
        /// </summary>
        /// <typeparam name="TMessage">Тип сообщения</typeparam>
        /// <param name="message">объект сообщения</param>
        Task Publish<TMessage>(TMessage message) where TMessage : IMessage;
    }
}
﻿using SM.Core;

namespace SM.Contracts.Commands
{
    /// <summary>
    /// Команда для подписки на тип сообщения
    /// </summary>
    public class UnsubscribeCommand : IMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string MessageType { get; set; }
    }
}
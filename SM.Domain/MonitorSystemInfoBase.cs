using System;
using SM.Model;

namespace SM.Domain
{
    public abstract class MonitorSystemInfoBase
    {
        /// <summary>
        /// Оповещает о высокой нагрузке
        /// </summary>
        public abstract event EventHandler OnHighLoaded;

        /// <summary>
        /// Системная информация
        /// </summary>
        public SystemInfo SystemInfo { get; } = new SystemInfo();

        /// <summary>
        /// Обновить состояние
        /// </summary>
        public abstract void Refresh();
    }
}
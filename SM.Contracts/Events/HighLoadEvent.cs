using SM.Core;
using SM.Model;

namespace SM.Contracts.Events
{
    /// <summary>
    /// Событие о высокой нагрузке в системе
    /// </summary>
    public class HighLoadEvent : EventMessage
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="systemInfo"></param>
        public HighLoadEvent(SystemInfo systemInfo) => SystemInfo = systemInfo;

        /// <summary>
        /// Системная информация
        /// </summary>
        public SystemInfo SystemInfo { get; set; }
    }
}

namespace SM.Model
{
    /// <summary>
    /// Состоянии процесса системы
    /// </summary>
    public class ProcessState
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="processName"></param>
        public ProcessState(int processId, string processName)
        {
            ProcessId = processId;
            ProcessName = processName;
        }

        /// <summary>
        /// Идентификатор процесса
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        /// Наименование процесса
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Процент использования CPU
        /// </summary>
        public double Cpu { get; set; }

        /// <summary>
        /// Количество используемой памяти в Mb
        /// </summary>
        public double Memory { get; set; }
    }
}
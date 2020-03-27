namespace SM.Model
{
    /// <summary>
    /// Системная информация
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Общий процент использования Cpu
        /// </summary>
        public double TotalCpuUsagePercent { get; set; }

        /// <summary>
        /// Общий процент использование оперативной памяти
        /// </summary>
        public double TotalMemoryUsagePercent { get; set; }

        /// <summary>
        /// Общее количество используемой оперативной памяти Mb 
        /// </summary>
        public float TotalMemoryUsageMb { get; set; }
    }
}
namespace Tang.Configurations
{
    public class LogConfig
    {
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string FilePath { get; set; } = "logs/log-.txt";

        /// <summary>
        /// 日志保留天数
        /// </summary>
        public int RetainedDays { get; set; } = 30;

        /// <summary>
        /// 是否输出到控制台
        /// </summary>
        public bool WriteToConsole { get; set; } = true;
    }
} 
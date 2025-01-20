namespace Tang.Configurations
{
    public class CacheConfig
    {
        /// <summary>
        /// 缓存类型(Memory/Redis)
        /// </summary>
        public string Type { get; set; } = "Memory";

        /// <summary>
        /// Redis连接字符串
        /// </summary>
        public string RedisConnection { get; set; } = string.Empty;
    }
} 
namespace Tang.Configurations
{
    public class JwtConfig
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// 过期时间(分钟)
        /// </summary>
        public int ExpireMinutes { get; set; }
    }
} 
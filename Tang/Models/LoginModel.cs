namespace Tang.Models
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>admin</example>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>123456</example>
        public string Password { get; set; } = string.Empty;
    }
} 
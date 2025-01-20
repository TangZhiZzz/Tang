using SqlSugar;

namespace Tang.Models
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    /// <example>admin</example>
    public class SysUser : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>admin</example>
        [SugarColumn(IsNullable = false)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>123456</example>
        [SugarColumn(IsNullable = false)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 状态(0:禁用,1:启用)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Remark { get; set; }
    }
} 
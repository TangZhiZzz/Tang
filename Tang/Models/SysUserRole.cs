namespace Tang.Models
{
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public class SysUserRole : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
    }
} 
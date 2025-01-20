namespace Tang.Models
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    public class SysRolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int PermissionId { get; set; }
    }
} 
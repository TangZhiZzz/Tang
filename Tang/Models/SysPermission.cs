using SqlSugar;

namespace Tang.Models
{
    /// <summary>
    /// 系统权限表
    /// </summary>
    public class SysPermission : BaseEntity
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 权限类型(1:菜单,2:按钮)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermissionCode { get; set; } = string.Empty;

        /// <summary>
        /// 路由地址
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        public string? Component { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态(0:禁用,1:启用)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 子权限
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<SysPermission>? Children { get; set; }
    }
} 
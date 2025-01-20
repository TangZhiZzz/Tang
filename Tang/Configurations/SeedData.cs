using Tang.Models;

namespace Tang.Configurations
{
    public static class SeedData
    {
        /// <summary>
        /// 初始化管理员角色
        /// </summary>
        public static SysRole AdminRole => new SysRole
        {
            RoleName = "超级管理员",
            RoleCode = "admin",
            Status = 1,
            Remark = "系统内置超级管理员",
            CreateTime = DateTime.Now,
            IsDeleted = false
        };

        /// <summary>
        /// 初始化管理员用户
        /// </summary>
        public static SysUser AdminUser => new SysUser
        {
            UserName = "admin",
            Password = "123456", // 实际应用中需要加密
            NickName = "超级管理员",
            Status = 1,
            Remark = "系统内置超级管理员账号",
            CreateTime = DateTime.Now,
            IsDeleted = false
        };

        /// <summary>
        /// 初始化基础权限
        /// </summary>
        public static List<SysPermission> BasicPermissions => new List<SysPermission>
        {
            new SysPermission
            {
                ParentId = 0,
                Name = "系统管理",
                Type = 1,
                PermissionCode = "system",
                Path = "/system",
                Component = "Layout",
                Icon = "setting",
                Sort = 1,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDeleted = false
            },
            new SysPermission
            {
                ParentId = 1,
                Name = "用户管理",
                Type = 1,
                PermissionCode = "system:user",
                Path = "user",
                Component = "/system/user/index",
                Icon = "user",
                Sort = 1,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDeleted = false
            },
            new SysPermission
            {
                ParentId = 1,
                Name = "角色管理",
                Type = 1,
                PermissionCode = "system:role",
                Path = "role",
                Component = "/system/role/index",
                Icon = "peoples",
                Sort = 2,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDeleted = false
            },
            new SysPermission
            {
                ParentId = 1,
                Name = "菜单管理",
                Type = 1,
                PermissionCode = "system:menu",
                Path = "menu",
                Component = "/system/menu/index",
                Icon = "tree-table",
                Sort = 3,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDeleted = false
            }
        };
    }
} 
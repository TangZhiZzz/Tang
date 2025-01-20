using System.Reflection;
using Microsoft.Extensions.Options;
using SqlSugar;
using Tang.Configurations;
using Tang.Models;

namespace Tang.Extensions
{
    public static class ServiceCollectionExtension
    {
        
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static IApplicationBuilder UseInitDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                
                // 创建数据库
                db.DbMaintenance.CreateDatabase();
                
                // 获取所有实体类型
                var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => 
                        !t.IsAbstract && 
                        !t.IsInterface && 
                        typeof(IEntity).IsAssignableFrom(t) && 
                        t != typeof(BaseEntity))  // 排除BaseEntity
                    .ToArray();
                
                // 创建表
                db.CodeFirst.InitTables(entityTypes);

                // 初始化数据
                InitData(db);
            }

            return app;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private static void InitData(ISqlSugarClient db)
        {
            // 判断是否已经初始化过
            if (db.Queryable<SysUser>().Any())
            {
                return;
            }

            // 启用事务
            db.Ado.BeginTran();
            try
            {
                // 初始化角色
                var roleId = db.Insertable(SeedData.AdminRole).ExecuteReturnIdentity();

                // 初始化用户
                var userId = db.Insertable(SeedData.AdminUser).ExecuteReturnIdentity();

                // 初始化用户角色关系
                db.Insertable(new SysUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                }).ExecuteCommand();

                // 初始化权限
                var permissions = SeedData.BasicPermissions;
                db.Insertable(permissions).ExecuteCommand();

                // 初始化角色权限关系
                var rolePermissions = permissions.Select(p => new SysRolePermission
                {
                    RoleId = roleId,
                    PermissionId = p.Id
                }).ToList();  // 转换为List类型
                
                db.Insertable(rolePermissions).ExecuteCommand();

                // 提交事务
                db.Ado.CommitTran();
            }
            catch
            {
                // 回滚事务
                db.Ado.RollbackTran();
                throw;
            }
        }
    }
} 
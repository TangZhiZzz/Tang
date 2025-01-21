using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Tang.Exceptions;
using Tang.Models;

namespace Tang.Controllers
{
    public class RoleController : BaseController
    {
        private readonly ISqlSugarClient _db;

        public RoleController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        [HttpGet("list")]
        public async Task<List<SysRole>> GetList([FromQuery] string? keyword)
        {
            var query = _db.Queryable<SysRole>()
                .WhereIF(!string.IsNullOrEmpty(keyword),
                    r => r.RoleName.Contains(keyword) || r.RoleCode.Contains(keyword))
                .Where(r => !r.IsDeleted);

            return await query.ToListAsync();

        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        [HttpGet("{id}")]
        public async Task<SysRole> GetInfo(int id)
        {
            var role = await _db.Queryable<SysRole>()
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstAsync();

            if (role == null)
                throw new ApiException("角色不存在");

            return role;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        [HttpPost]
        public async Task Add([FromBody] SysRole role)
        {
            // 检查角色编码是否已存在
            if (await _db.Queryable<SysRole>().AnyAsync(r => r.RoleCode == role.RoleCode && !r.IsDeleted))
                throw new ApiException("角色编码已存在");

            var result = await _db.Insertable(role).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [HttpPut]
        public async Task Update([FromBody] SysRole role)
        {
            // 检查角色是否存在
            if (!await _db.Queryable<SysRole>().AnyAsync(r => r.Id == role.Id && !r.IsDeleted))
                throw new ApiException("角色不存在");

            // 检查角色编码是否已被其他角色使用
            if (await _db.Queryable<SysRole>().AnyAsync(r =>
                r.RoleCode == role.RoleCode && r.Id != role.Id && !r.IsDeleted))
                throw new ApiException("角色编码已存在");

            role.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(role).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var role = await _db.Queryable<SysRole>()
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstAsync();

            if (role == null)
                throw new ApiException("角色不存在");

            // 检查是否有用户使用此角色
            if (await _db.Queryable<SysUserRole>().AnyAsync(ur => ur.RoleId == id))
                throw new ApiException("角色已被用户使用，无法删除");

            // 软删除
            role.IsDeleted = true;
            role.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(role).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        [HttpGet("{roleId}/permissions")]
        public async Task<List<SysPermission>> GetRolePermissions(int roleId)
        {
            var permissions = await _db.Queryable<SysPermission>()
                .InnerJoin<SysRolePermission>((p, rp) => p.Id == rp.PermissionId)
                .Where((p, rp) => rp.RoleId == roleId && !p.IsDeleted)
                .Select(p => p)
                .ToListAsync();

            return permissions;
        }

        /// <summary>
        /// 设置角色权限
        /// </summary>
        [HttpPost("{roleId}/permissions")]
        public async Task SetRolePermissions(int roleId, [FromBody] List<int> permissionIds)
        {
            // 检查角色是否存在
            if (!await _db.Queryable<SysRole>().AnyAsync(r => r.Id == roleId && !r.IsDeleted))
                throw new ApiException("角色不存在");


            // 删除原有权限关系
            await _db.Deleteable<SysRolePermission>().Where(rp => rp.RoleId == roleId).ExecuteCommandAsync();

            // 添加新的权限关系
            var rolePermissions = permissionIds.Select(permissionId => new SysRolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(rolePermissions).ExecuteCommandAsync();


        }
    }
}
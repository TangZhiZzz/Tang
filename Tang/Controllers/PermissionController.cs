using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Tang.Exceptions;
using Tang.Models;

namespace Tang.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly ISqlSugarClient _db;

        public PermissionController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        [HttpGet("tree")]
        public async Task<List<SysPermission>> GetTree([FromQuery] string? keyword)
        {
            var permissions = await _db.Queryable<SysPermission>()
                .WhereIF(!string.IsNullOrEmpty(keyword), 
                    p => p.Name.Contains(keyword) || p.PermissionCode.Contains(keyword))
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Sort)
                .ToListAsync();

            // 构建树形结构
            return BuildTree(permissions);
        }

        /// <summary>
        /// 获取权限信息
        /// </summary>
        [HttpGet("{id}")]
        public async Task<SysPermission> GetInfo(int id)
        {
            var permission = await _db.Queryable<SysPermission>()
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstAsync();

            if (permission == null)
            {
                throw new ApiException("权限不存在");
            }
                 

            return permission;
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        [HttpPost]
        public async Task Add([FromBody] SysPermission permission)
        {
            // 检查权限编码是否已存在
            if (await _db.Queryable<SysPermission>().AnyAsync(p => p.PermissionCode == permission.PermissionCode && !p.IsDeleted))
                throw new ApiException("权限编码已存在");

            var result = await _db.Insertable(permission).ExecuteCommandAsync();
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        [HttpPut]
        public async Task Update([FromBody] SysPermission permission)
        {
            // 检查权限是否存在
            if (!await _db.Queryable<SysPermission>().AnyAsync(p => p.Id == permission.Id && !p.IsDeleted))
                throw new ApiException("权限不存在");

            // 检查权限编码是否已被其他权限使用
            if (await _db.Queryable<SysPermission>().AnyAsync(p => 
                p.PermissionCode == permission.PermissionCode && p.Id != permission.Id && !p.IsDeleted))
                throw new ApiException("权限编码已存在");

            permission.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(permission).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var permission = await _db.Queryable<SysPermission>()
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstAsync();

            if (permission == null)
                throw new ApiException("权限不存在");

            // 检查是否有子权限
            if (await _db.Queryable<SysPermission>().AnyAsync(p => p.ParentId == id && !p.IsDeleted))
                throw new ApiException("存在子权限，无法删除");

            // 检查是否有角色使用此权限
            if (await _db.Queryable<SysRolePermission>().AnyAsync(rp => rp.PermissionId == id))
                throw new ApiException("权限已被角色使用，无法删除");

            // 软删除
            permission.IsDeleted = true;
            permission.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(permission).ExecuteCommandAsync();
        }

        /// <summary>
        /// 构建权限树
        /// </summary>
        private List<SysPermission> BuildTree(List<SysPermission> permissions, int parentId = 0)
        {
            return permissions
                .Where(p => p.ParentId == parentId)
                .Select(p =>
                {
                    p.Children = BuildTree(permissions, p.Id);
                    return p;
                })
                .ToList();
        }
    }
} 
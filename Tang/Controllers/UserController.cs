using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Tang.Models;
using Microsoft.AspNetCore.Authorization;
using Tang.Services;
using Tang.Exceptions;
using Tang.Extensions;

namespace Tang.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Authorize]
    public class UserController : BaseController
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogService _log;

        public UserController(ISqlSugarClient db, ILogService log)
        {
            _db = db;
            _log = log;
        }
        
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="keyword">搜索关键词(用户名/昵称)</param>
        /// <returns>用户列表</returns>
        [HttpGet("list")]
        public async Task<List<SysUser>> GetList([FromQuery] string? keyword)
        {
            var query = _db.Queryable<SysUser>()
                .WhereIF(!string.IsNullOrEmpty(keyword),
                    u => u.UserName.Contains(keyword) || u.NickName!.Contains(keyword))
                .Where(u => !u.IsDeleted);

            return await query.ToListAsync();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        public async Task<SysUser> GetInfo(int id)
        {
            var user = await _db.Queryable<SysUser>()
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstAsync();

            if (user == null)
                throw new ApiException("用户不存在");

            return user;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        [HttpPost]
        public async Task Add([FromBody] SysUser user)
        {

            if (await _db.Queryable<SysUser>().AnyAsync(u => u.UserName == user.UserName && !u.IsDeleted))
            {
                throw new ApiException("用户名已存在");
            }

            var result = await _db.Insertable(user).ExecuteCommandAsync();

        }

        /// <summary>
        /// 修改用户
        /// </summary>
        [HttpPut]
        public async Task Update([FromBody] SysUser user)
        {
            // 检查用户是否存在
            if (!await _db.Queryable<SysUser>().AnyAsync(u => u.Id == user.Id && !u.IsDeleted))
                throw new ApiException("用户不存在");

            // 检查用户名是否已被其他用户使用
            if (await _db.Queryable<SysUser>().AnyAsync(u =>
                u.UserName == user.UserName && u.Id != user.Id && !u.IsDeleted))
                throw new ApiException("用户名已存在");

            user.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(user).ExecuteCommandAsync();

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var user = await _db.Queryable<SysUser>()
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstAsync();

            if (user == null)
                throw new ApiException("用户不存在");

            // 软删除
            user.IsDeleted = true;
            user.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(user).ExecuteCommandAsync();

        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        [HttpGet("{userId}/roles")]
        public async Task<List<SysRole>> GetUserRoles(int userId)
        {
            var roles = await _db.Queryable<SysRole>()
                .InnerJoin<SysUserRole>((r, ur) => r.Id == ur.RoleId)
                .Where((r, ur) => ur.UserId == userId && !r.IsDeleted)
                .Select(r => r)
                .ToListAsync();

            return roles;
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        [HttpPost("{userId}/roles")]
        public async Task SetUserRoles(int userId, [FromBody] List<int> roleIds)
        {
            // 检查用户是否存在
            if (!await _db.Queryable<SysUser>().AnyAsync(u => u.Id == userId && !u.IsDeleted))
                throw new ApiException("用户不存在");


            // 删除原有角色关系
            await _db.Deleteable<SysUserRole>().Where(ur => ur.UserId == userId).ExecuteCommandAsync();

            // 添加新的角色关系
            var userRoles = roleIds.Select(roleId => new SysUserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(userRoles).ExecuteCommandAsync();

        }

        /// <summary>
        /// 分页获取用户列表
        /// </summary>
        /// <param name="request">分页请求参数</param>
        /// <param name="keyword">搜索关键词(用户名/昵称)</param>
        /// <returns>分页用户列表</returns>
        [HttpGet("page")]
        public async Task<PageResult<SysUser>> GetPage([FromQuery] PageRequest request, [FromQuery] string? keyword)
        {
            var query = _db.Queryable<SysUser>()
                .WhereIF(!string.IsNullOrEmpty(keyword),
                    u => u.UserName.Contains(keyword) || u.NickName!.Contains(keyword))
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreateTime); // 按创建时间倒序
            return query.ToPageResult<SysUser>(request);
        }
    }
}
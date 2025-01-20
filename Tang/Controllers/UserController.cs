using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Tang.Models;
using Microsoft.AspNetCore.Authorization;
using Tang.Services;

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
        public async Task<IActionResult> GetList([FromQuery] string? keyword)
        {
            var query = _db.Queryable<SysUser>()
                .WhereIF(!string.IsNullOrEmpty(keyword), 
                    u => u.UserName.Contains(keyword) || u.NickName!.Contains(keyword))
                .Where(u => !u.IsDeleted);

            var list = await query.ToListAsync();
            return Success(list);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfo(int id)
        {
            var user = await _db.Queryable<SysUser>()
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstAsync();

            if (user == null)
                return Error("用户不存在");

            return Success(user);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SysUser user)
        {
            try
            {
                _log.Info("添加用户: {UserName}", user.UserName);

                if (await _db.Queryable<SysUser>().AnyAsync(u => u.UserName == user.UserName && !u.IsDeleted))
                {
                    _log.Warning("用户名已存在: {UserName}", user.UserName);
                    return Error("用户名已存在");
                }

                var result = await _db.Insertable(user).ExecuteCommandAsync();
                
                _log.Info("用户添加成功: {UserName}", user.UserName);
                return result > 0 ? Success() : Error("添加失败");
            }
            catch (Exception ex)
            {
                _log.Error("添加用户失败: {UserName}", ex, user.UserName);
                return Error("添加失败");
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SysUser user)
        {
            // 检查用户是否存在
            if (!await _db.Queryable<SysUser>().AnyAsync(u => u.Id == user.Id && !u.IsDeleted))
                return Error("用户不存在");

            // 检查用户名是否已被其他用户使用
            if (await _db.Queryable<SysUser>().AnyAsync(u => 
                u.UserName == user.UserName && u.Id != user.Id && !u.IsDeleted))
                return Error("用户名已存在");

            user.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(user).ExecuteCommandAsync();
            return result > 0 ? Success() : Error("修改失败");
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Queryable<SysUser>()
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstAsync();

            if (user == null)
                return Error("用户不存在");

            // 软删除
            user.IsDeleted = true;
            user.UpdateTime = DateTime.Now;
            var result = await _db.Updateable(user).ExecuteCommandAsync();
            return result > 0 ? Success() : Error("删除失败");
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var roles = await _db.Queryable<SysRole>()
                .InnerJoin<SysUserRole>((r, ur) => r.Id == ur.RoleId)
                .Where((r, ur) => ur.UserId == userId && !r.IsDeleted)
                .Select(r => r)
                .ToListAsync();

            return Success(roles);
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> SetUserRoles(int userId, [FromBody] List<int> roleIds)
        {
            // 检查用户是否存在
            if (!await _db.Queryable<SysUser>().AnyAsync(u => u.Id == userId && !u.IsDeleted))
                return Error("用户不存在");

            // 开启事务
            try
            {
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

                return Success();
            }
            catch (Exception)
            {
                return Error("设置角色失败");
            }
        }
    }
} 
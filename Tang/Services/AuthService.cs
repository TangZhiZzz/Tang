using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using Tang.Configurations;
using Tang.Models;

namespace Tang.Services
{
    public class AuthService : IAuthService, IScoped
    {
        private readonly ISqlSugarClient _db;
        private readonly JwtConfig _jwtConfig;

        public AuthService(ISqlSugarClient db, IOptions<JwtConfig> jwtConfig)
        {
            _db = db;
            _jwtConfig = jwtConfig.Value;
        }

        /// <summary>
        /// 登录
        /// </summary>
        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _db.Queryable<SysUser>()
                .Where(u => u.UserName == username && u.Password == password && !u.IsDeleted)
                .FirstAsync();

            if (user == null)
                return null;

            // 获取用户角色
            var roles = await _db.Queryable<SysRole>()
                .InnerJoin<SysUserRole>((r, ur) => r.Id == ur.RoleId)
                .Where((r, ur) => ur.UserId == user.Id && !r.IsDeleted)
                .Select(r => r.RoleCode)
                .ToListAsync();

            // 获取用户权限
            var permissions = await _db.Queryable<SysPermission>()
                .InnerJoin<SysRolePermission>((p, rp) => p.Id == rp.PermissionId)
                .InnerJoin<SysUserRole>((p, rp, ur) => rp.RoleId == ur.RoleId)
                .Where((p, rp, ur) => ur.UserId == user.Id && !p.IsDeleted)
                .Select(p => p.PermissionCode)
                .ToListAsync();

            // 生成Token
            return GenerateToken(user, roles, permissions);
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        private string GenerateToken(SysUser user, List<string> roles, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 添加角色
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 添加权限
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.ExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 
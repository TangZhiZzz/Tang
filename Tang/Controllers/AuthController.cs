using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tang.Models;
using Tang.Services;

namespace Tang.Controllers
{
    /// <summary>
    /// 认证管理
    /// </summary>
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model">登录信息</param>
        /// <returns>Token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.LoginAsync(model.UserName, model.Password);
            if (token == null)
                return Error("用户名或密码错误");

            return Success(new { token });
        }
    }
} 
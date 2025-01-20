using Microsoft.AspNetCore.Mvc;

namespace Tang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 返回成功
        /// </summary>
        protected IActionResult Success() => Ok();

        /// <summary>
        /// 返回成功带数据
        /// </summary>
        protected IActionResult Success<T>(T data) => Ok(data);

        /// <summary>
        /// 返回错误消息
        /// </summary>
        protected IActionResult Error(string message) => BadRequest(message);
    }
} 
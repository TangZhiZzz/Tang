using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tang.Services;

namespace Tang.Controllers
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    [Authorize]
    public class CacheController : BaseController
    {
        private readonly ICacheService _cache;

        public CacheController(ICacheService cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            if (await _cache.ExistsAsync(key))
            {
                var value = await _cache.GetAsync<object>(key);
                return Success(value);
            }

            return Error("缓存不存在");
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">过期时间(分钟)</param>
        [HttpPost("{key}")]
        public async Task<IActionResult> Set(string key, [FromBody] object value, [FromQuery] int? expiry = null)
        {
            TimeSpan? expiryTimeSpan = expiry.HasValue ? TimeSpan.FromMinutes(expiry.Value) : null;
            await _cache.SetAsync(key, value, expiryTimeSpan);
            return Success();
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        [HttpDelete("{key}")]
        public async Task<IActionResult> Remove(string key)
        {
            if (await _cache.ExistsAsync(key))
            {
                await _cache.RemoveAsync(key);
                return Success();
            }

            return Error("缓存不存在");
        }

        /// <summary>
        /// 检查缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        [HttpGet("exists/{key}")]
        public async Task<IActionResult> Exists(string key)
        {
            var exists = await _cache.ExistsAsync(key);
            return Success(exists);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _cache.ClearAsync();
            return Success();
        }

        /// <summary>
        /// 批量获取缓存
        /// </summary>
        /// <param name="keys">缓存键列表</param>
        [HttpPost("batch/get")]
        public async Task<IActionResult> BatchGet([FromBody] List<string> keys)
        {
            var result = new Dictionary<string, object?>();
            foreach (var key in keys)
            {
                if (await _cache.ExistsAsync(key))
                {
                    result[key] = await _cache.GetAsync<object>(key);
                }
            }
            return Success(result);
        }

        /// <summary>
        /// 批量移除缓存
        /// </summary>
        /// <param name="keys">缓存键列表</param>
        [HttpPost("batch/remove")]
        public async Task<IActionResult> BatchRemove([FromBody] List<string> keys)
        {
            foreach (var key in keys)
            {
                if (await _cache.ExistsAsync(key))
                {
                    await _cache.RemoveAsync(key);
                }
            }
            return Success();
        }
    }
} 
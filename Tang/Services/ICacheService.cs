namespace Tang.Services
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// 是否存在
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 清空缓存
        /// </summary>
        Task ClearAsync();
    }
} 
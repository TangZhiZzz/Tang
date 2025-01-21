namespace Tang.Models
{
    /// <summary>
    /// 分页请求参数
    /// </summary>
    public class PageRequest
    {
        private int _pageSize = 10;
        private int _pageIndex = 1;

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value < 1 ? 1 : value;
        }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value;
        }
    }
} 
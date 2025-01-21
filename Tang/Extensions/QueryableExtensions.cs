using SqlSugar;
using Tang.Models;

namespace Tang.Extensions
{
    /// <summary>
    /// IQueryable扩展方法
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 转换为分页结果
        /// </summary>
        public static PageResult<T> ToPageResult<T>(this IQueryable<T> query, PageRequest pageRequest)
        {
            var total = query.Count();
            var items = query.Skip((pageRequest.PageIndex - 1) * pageRequest.PageSize)
                           .Take(pageRequest.PageSize)
                           .ToList();

            return new PageResult<T>
            {
                PageIndex = pageRequest.PageIndex,
                PageSize = pageRequest.PageSize,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageRequest.PageSize),
                Items = items
            };
        }

        /// <summary>
        /// 转换为分页结果
        /// </summary>
        public static PageResult<T> ToPageResult<T>(this ISugarQueryable<T> query, PageRequest pageRequest)
        {
            var total = query.Count();
            var items = query.Skip((pageRequest.PageIndex - 1) * pageRequest.PageSize)
                           .Take(pageRequest.PageSize)
                           .ToList();

            return new PageResult<T>
            {
                PageIndex = pageRequest.PageIndex,
                PageSize = pageRequest.PageSize,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageRequest.PageSize),
                Items = items
            };
        }
    }
} 
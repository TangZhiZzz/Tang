using SqlSugar;
using Tang.Models;

namespace Tang.Repositories
{
    public class BaseRepository<T> where T : BaseEntity, new()
    {
        protected readonly ISqlSugarClient _db;

        public BaseRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        protected ISugarQueryable<T> Entities => _db.Queryable<T>().Where(x => !x.IsDeleted);
    }
} 
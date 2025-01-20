namespace Tang.Configurations
{
    public class DbConfig
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public SqlSugar.DbType DbType { get; set; } = SqlSugar.DbType.Sqlite;

        /// <summary>
        /// 是否自动关闭连接
        /// </summary>
        public bool IsAutoCloseConnection { get; set; } = true;
    }
} 
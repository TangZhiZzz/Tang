namespace Tang.Services
{
    /// <summary>
    /// 瞬时服务
    /// </summary>
    public interface ITransient : IServiceRegister { }

    /// <summary>
    /// 作用域服务
    /// </summary>
    public interface IScoped : IServiceRegister { }

    /// <summary>
    /// 单例服务
    /// </summary>
    public interface ISingleton : IServiceRegister { }
} 
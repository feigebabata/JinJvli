namespace FGUFW.Core.System
{
    /// <summary>
    /// 管理IUnit的创建 获取 释放
    /// </summary>
    public interface IUnitManager
    {
        T Unit<T>() where T:IUnit;
    }

    /// <summary>
    /// 配合IUnitManager使用
    /// </summary>
    public interface IUnit
    {
        void Release();
    }
}
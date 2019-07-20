namespace JinJvli
{
    public interface ISendData
    {
        byte[] Pack();
    }

    public interface IReceveData
    {
        T Unpack<T>(byte[] _data);
    }
}
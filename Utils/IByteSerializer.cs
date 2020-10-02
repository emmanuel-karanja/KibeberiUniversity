namespace KibeberiUniversity.Utils
{
    public interface IByteSerializer
    {
         T Deserialize<T>(byte[] bytes);
         byte[] Serialize<T> (T obj);
    }

}
using System.Text;
using Newtonsoft.Json;

namespace KibeberiUniversity.Utils
{
    public class ByteSerializer
    {
        public T Deserialize<T> (byte[] bytes)=> JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(bytes));
        public byte[] Serialize<T>(T obj)=>Encoding.Default.GetBytes(JsonConvert.SerializeObject(obj));
    }
}
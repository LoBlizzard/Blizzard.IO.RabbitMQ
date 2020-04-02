using Blizzard.IO.Core;
using Newtonsoft.Json;
using System.Text;

namespace Blizzard.IO.RabbitMQ
{
    public class JsonDeserializer<TData> : IDeserializer<TData>
    {
        public TData Deserialize(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<TData>(Encoding.UTF8.GetString(bytes));
        }
    }
}

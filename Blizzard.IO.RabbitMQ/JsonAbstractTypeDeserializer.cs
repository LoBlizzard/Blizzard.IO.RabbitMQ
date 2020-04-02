using Blizzard.IO.Core;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Blizzard.IO.RabbitMQ
{
    public class JsonAbstractTypeDeserializer<TData> : IConcreteTypeDeserializer<TData>
    {
        public TData Deserialize(byte[] bytes, Type type)
        {
            return (TData) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type);
        }
    }
}

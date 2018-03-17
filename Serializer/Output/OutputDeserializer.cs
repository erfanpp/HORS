using Newtonsoft.Json;
using NHORS.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace NHORS.Serializer.Output
{
    public class OutputDeserializer : IOutputDeserializer
    {
        public async Task<U> Deserialize<U>(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return await Task.Run(() => serializer.Deserialize<U>(reader));                
            }
        }
    }
}

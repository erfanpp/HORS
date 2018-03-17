using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NHORS.Serializer.Input
{
    public class InputSerializer : IInputSerializer
    {
        public async Task<StringContent> SerializeAsync<U>(U body, string contentType = null, Encoding encoding = null)
        {
            return new StringContent(await Task.Run(() => JsonConvert.SerializeObject(body)), encoding, contentType);
        }
    }
}

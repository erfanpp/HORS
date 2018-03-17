using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NHORS.Serializer.Input
{
    public interface IInputSerializer
    {
        Task<StringContent> SerializeAsync<U>(U body, string contentType = null, Encoding encoding = null);
    }
}

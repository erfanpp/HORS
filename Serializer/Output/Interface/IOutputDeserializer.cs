using System.IO;
using System.Threading.Tasks;

namespace NHORS.Serializer.Output
{
    public interface IOutputDeserializer
    {
        Task<U> Deserialize<U>(Stream stream);
    }
}

using NHORS.Serializer.Input;
using NHORS.Serializer.Output;

namespace NHORS.Serializer.Serializar
{
    public interface ISerializerHandling
    {
        InputSerializer InputSerializer { get; set; }
        OutputDeserializer OutputDeserializer { get; set; }
    }
}
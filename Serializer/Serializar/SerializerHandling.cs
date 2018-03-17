using NHORS.Serializer.Input;
using NHORS.Serializer.Output;

namespace NHORS.Serializer.Serializar
{
    public class SerializerHandling : ISerializerHandling
    {
        public SerializerHandling(InputSerializer inputSerializer,OutputDeserializer outputDeserializer)
        {
            InputSerializer = inputSerializer;
            OutputDeserializer = outputDeserializer;
        }
        public SerializerHandling(InputSerializer inputSerializer)
        {
            InputSerializer = inputSerializer;            
        }
        public SerializerHandling(OutputDeserializer outputDeserializer)
        {
            OutputDeserializer = outputDeserializer;
        }
        public InputSerializer InputSerializer { get; set; }
        public OutputDeserializer OutputDeserializer { get; set; }
    }
}

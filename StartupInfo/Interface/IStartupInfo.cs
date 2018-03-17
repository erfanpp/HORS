using System;
using NHORS.Event.EventHandling;
using NHORS.Event.InputEvent;
using NHORS.NInteraction;
using NHORS.Serializer.Input;
using NHORS.Serializer.Output;
using NHORS.Serializer.Serializar;

namespace NHORS.NStartupInfo
{
    public interface IStartupInfo
    {
        Tuple<string, string> AuthenticateToken { get; set; }
        IEventHandling EventHandling { get; set; }
        IInputEvent InputEvent { get; set; }
        Interaction InputEventInteraction { get; set; }
        IInputSerializer InputSerializer { get; set; }
        IOutputDeserializer OutputDeserializer { get; set; }
        IOutputEvent OutputEvent { get; set; }
        Interaction OutputEventInteraction { get; set; }
        string Port { get; set; }
        ISerializerHandling SerializerHandling { get; set; }
        string ServerAddress { get; set; }
    }
}
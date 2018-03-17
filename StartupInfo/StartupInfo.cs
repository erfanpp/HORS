using NHORS.Event.EventHandling;
using NHORS.Event.InputEvent;
using NHORS.NInteraction;
using NHORS.Serializer.Input;
using NHORS.Serializer.Output;
using NHORS.Serializer.Serializar;
using System;

namespace NHORS.NStartupInfo
{
    public class StartupInfo : IStartupInfo
    {
        public string ServerAddress { get; set; }
        public string Port { get; set; }
        public Tuple<string,string> AuthenticateToken { get; set; }
        public ISerializerHandling SerializerHandling { get; set; }
        public IInputSerializer InputSerializer { get; set; }
        public IOutputDeserializer OutputDeserializer { get; set; }
        public IEventHandling EventHandling { get; set; }
        public IInputEvent InputEvent { get; set; }
        public IOutputEvent OutputEvent { get; set; }
        public Interaction InputEventInteraction { get; set; } = Interaction.Dontinteract;
        public Interaction OutputEventInteraction { get; set; } = Interaction.Dontinteract;
    }
}

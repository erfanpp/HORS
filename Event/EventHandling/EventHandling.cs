using NHORS.Event.InputEvent;

namespace NHORS.Event.EventHandling
{
    public class EventHandling : IEventHandling
    {
        public IInputEvent InputEvent { get; set; }
        public IOutputEvent OutputEvent { get; set; }
    }
}

using NHORS.Event.InputEvent;

namespace NHORS.Event.EventHandling
{
    public interface IEventHandling
    {
        IInputEvent InputEvent { get; set; }
        IOutputEvent OutputEvent { get; set; }
    }
}

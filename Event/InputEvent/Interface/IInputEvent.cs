using System.Net.Http;

namespace NHORS.Event.InputEvent
{
    public interface IInputEvent
    {
        HttpRequestMessage Invoke(HttpRequestMessage httpRequestMessage);
    }
}

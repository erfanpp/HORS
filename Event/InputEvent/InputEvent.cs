using System.Net.Http;

namespace NHORS.Event.InputEvent
{
    public class InputEvent : IInputEvent
    {
        public HttpRequestMessage Invoke(HttpRequestMessage httpRequestMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}

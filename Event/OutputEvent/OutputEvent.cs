using System.Net.Http;
using NHORS.Event.InputEvent;

namespace NHORS.Event.OutputEvent
{
    public class OutputEvent : IOutputEvent
    {
        public HttpResponseMessage Invoke(HttpResponseMessage httpResponseMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}

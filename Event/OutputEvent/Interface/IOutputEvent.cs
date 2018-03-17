using System.Net.Http;

namespace NHORS.Event.InputEvent
{
    public interface IOutputEvent
    {
        HttpResponseMessage Invoke(HttpResponseMessage httpRequestMessage);
    }
}

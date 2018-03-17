using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHORS.Event.EventHandling;
using NHORS.Event.InputEvent;
using NHORS.NInteraction;
using NHORS.Serializer.Input;
using NHORS.Serializer.Output;
using NHORS.Serializer.Serializar;

namespace NHORS.NHORS
{
    public interface IHORS
    {
        IOutputDeserializer _deserializerOutput { get; set; }
        HttpContent _httpContent { get; set; }
        HttpHeaders _httpHeaders { get; set; }
        HttpMethod _httpMethod { get; set; }
        IInputEvent _inputEvent { get; set; }
        Interaction _inputEventDuty { get; set; }
        IOutputEvent _outputEvent { get; set; }
        Interaction _outputEventDuty { get; set; }
        IInputSerializer _serializerInput { get; set; }
        Uri _uri { get; set; }

        HORS AddHttpHeader(string key, string value);
        HORS AddRequestSegment(string Segment);
        Task<HttpResponseMessage> DeleteAsync();
        Task<HttpResponseMessage> DeleteAsync(CancellationToken cancellationToken);
        Task<HttpContent> DeleteBodyAsync();
        Task<HttpContent> DeleteBodyAsync(CancellationToken cancellationToken);
        Task<U> DeleteDeserializeAsync<U>();
        Task<U> DeleteDeserializeAsync<U>(CancellationToken cancellationToken);
        Task<(U, HttpResponseMessage)> DeleteDeserializeRawAsync<U>();
        Task<(U, HttpResponseMessage)> DeleteDeserializeRawAsync<U>(CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync();
        Task<HttpResponseMessage> GetAsync(CancellationToken cancellationToken);
        Task<HttpContent> GetBodyAsync();
        Task<HttpContent> GetBodyAsync(CancellationToken cancellationToken);
        Task<U> GetDeserializeAsync<U>();
        Task<U> GetDeserializeAsync<U>(CancellationToken cancellationToken);
        Task<(U, HttpResponseMessage)> GetDeserializeRawAsync<U>();
        Task<(U, HttpResponseMessage)> GetDeserializeRawAsync<U>(CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync();
        Task<HttpResponseMessage> PostAsync(CancellationToken cancellationToken);
        Task<HttpContent> PostBodyAsync();
        Task<HttpContent> PostBodyAsync(CancellationToken cancellationToken);
        Task<U> PostDeserializeAsync<U>();
        Task<U> PostDeserializeAsync<U>(CancellationToken cancellationToken);
        Task<(U, HttpResponseMessage)> PostDeserializeRawAsync<U>();
        Task<(U, HttpResponseMessage)> PostDeserializeRawAsync<U>(CancellationToken cancellationToken);
        Task<HttpResponseMessage> PutAsync();
        Task<HttpResponseMessage> PutAsync(CancellationToken cancellationToken);
        Task<HttpContent> PutBodyAsync();
        Task<HttpContent> PutBodyAsync(CancellationToken cancellationToken);
        Task<U> PutDeserializeAsync<U>();
        Task<U> PutDeserializeAsync<U>(CancellationToken cancellationToken);
        Task<(U, HttpResponseMessage)> PutDeserializeRawAsync<U>();
        Task<(U, HttpResponseMessage)> PutDeserializeRawAsync<U>(CancellationToken cancellationToken);
        HORS SetBaseAddress(string Server);
        HORS SetEventHandling(IEventHandling eventHandling);
        HORS SetEventHandlingDuty(Interaction InputInteraction, Interaction OutputInteraction);
        HORS SetHttpMethod(HttpMethod httpMethod);
        HORS SetInputEvent(IInputEvent inputEvent);
        HORS SetInputSerializer(IInputSerializer serializerInput);
        HORS SetOutputDeserializer(IOutputDeserializer deserializerOutput);
        HORS SetOutputEvent(IOutputEvent outputEvent);
        HORS SetRequestPath(string Path);
        HORS SetSerializer(ISerializerHandling serializar);
        HORS SetStringBody<U>(U body, string contentType = "Application/Json", Encoding encoding = null);
    }
}
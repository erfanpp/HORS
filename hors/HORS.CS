using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHORS.Core;
using NHORS.Event.EventHandling;
using NHORS.Event.InputEvent;
using NHORS.NInteraction;
using NHORS.NStartupInfo;
using NHORS.Serializer.Input;
using NHORS.Serializer.Output;
using NHORS.Serializer.Serializar;
using NHORS.Helpers;

namespace NHORS.NHORS
{
    public sealed class HORS : HORSBase,IHORS,IDisposable
    {
        #region Properties
        public Uri _uri { get; set; }
        public HttpHeaders _httpHeaders { get; set; }
        public HttpContent _httpContent { get; set; }
        public HttpMethod _httpMethod { get; set; }
        public IInputSerializer _serializerInput { get; set; }
        public IOutputDeserializer _deserializerOutput { get; set; }
        public IInputEvent _inputEvent { get; set; }
        public IOutputEvent _outputEvent { get; set; }
        public Interaction _inputEventDuty { get; set; }
        public Interaction _outputEventDuty { get; set; }
        #endregion    
        #region Ctor       
        public HORS(Uri uri,HttpClient httpClient) : base(uri,httpClient)
        {
            _uri = uri;
        }
        public HORS(IStartupInfo startupInfo,HttpClient httpClient) : base(startupInfo,httpClient)
        {
            SetBaseAddress(startupInfo.ServerAddress);

            if (startupInfo.AuthenticateToken != null)
                AddHttpHeader(startupInfo.AuthenticateToken.Item1, startupInfo.AuthenticateToken.Item2);

            if (startupInfo.InputEventInteraction != Interaction.Default)
                _inputEventDuty = startupInfo.InputEventInteraction;

            if (startupInfo.OutputEventInteraction != Interaction.Default)
                _outputEventDuty = startupInfo.OutputEventInteraction;

            if (startupInfo.EventHandling != null ||
               (startupInfo.InputEvent != null) || (startupInfo.OutputEvent != null))
            {
                if(startupInfo.InputEvent != null)
                _inputEvent = startupInfo.InputEvent ?? startupInfo.EventHandling.InputEvent;
                if(startupInfo.OutputEvent != null)
                _outputEvent = startupInfo.OutputEvent ?? startupInfo.EventHandling.OutputEvent;
            }
            if (startupInfo.SerializerHandling != null ||
               (startupInfo.InputSerializer != null || startupInfo.OutputDeserializer != null))
             {
                if(startupInfo.InputSerializer != null)
                _serializerInput = startupInfo.InputSerializer ?? startupInfo.SerializerHandling.InputSerializer;
                if (startupInfo.OutputDeserializer != null)
                    _deserializerOutput = startupInfo.OutputDeserializer ?? startupInfo.SerializerHandling.OutputDeserializer;
            }
        }
        #endregion
        #region Settings      
        public HORS SetHttpMethod(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
            return this;
        }
        public HORS SetBaseAddress(string Server)
        {
            _uri = new Uri(Server);
            return this;
        }
        public HORS SetRequestPath(string Path)
        {
            _uri = new Uri($"{_uri.ToString()}/{Path}");
            return this;
        }
        public HORS AddRequestSegment(string Segment)
        {
            _uri = new Uri($"{_uri.AbsoluteUri}/{Segment}");
            return this;
        }
        public HORS AddHttpHeader(string key, string value)
        {
            if (_httpHeaders.TryAddWithoutValidation(key, value))
                return this;
            throw new Exception($"{nameof(key)} with the value of {value} could not be added");
        }
        public HORS SetStringBody<U>(U body, string contentType = "Application/Json", Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            if (body.Equals(null))
                throw new NullReferenceException(nameof(body));
            if (!_serializerInput.Equals(null))
            {                                               
                _httpContent = AsyncHelpers.RunSync(() => _serializerInput.SerializeAsync(body, contentType, encoding));
                return this;
            }
            else if (!string.IsNullOrWhiteSpace(body.ToString()))
                _httpContent = new StringContent(body.ToString(), encoding, contentType);
            else
                throw new NullReferenceException(nameof(_serializerInput));
            return this;
        }
        public HORS SetInputSerializer(IInputSerializer serializerInput)
        {
            _serializerInput = serializerInput;
            return this;
        }
        public HORS SetOutputDeserializer(IOutputDeserializer deserializerOutput)
        {
            _deserializerOutput = deserializerOutput;
            return this;
        }
        public HORS SetSerializer(ISerializerHandling serializar)
        {
            _serializerInput = serializar.InputSerializer;
            _deserializerOutput = serializar.OutputDeserializer;
            return this;
        }
        public HORS SetEventHandling(IEventHandling eventHandling)
        {
            _inputEvent = eventHandling.InputEvent;
            _outputEvent = eventHandling.OutputEvent;
            return this;
        }
        public HORS SetInputEvent(IInputEvent inputEvent)
        {
            _inputEvent = inputEvent;
            return this;
        }
        public HORS SetOutputEvent(IOutputEvent outputEvent)
        {
            _outputEvent = outputEvent;
            return this;
        }
        public HORS SetEventHandlingDuty(Interaction InputInteraction, Interaction OutputInteraction)
        {
            _inputEventDuty = InputInteraction;
            _outputEventDuty = OutputInteraction;
            return this;
        }
        #endregion
        #region DataLoaders
        #region Internals
        internal async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod)
        {            
            _httpMethod = httpMethod;
            using (var request = await SetupHttpRequestMessage(this))
            {
                var result = await Send(request);
                if (result.IsSuccessStatusCode)
                    if (_outputEventDuty != Interaction.Dontinteract)
                        return _outputEvent.Invoke(result);
                    else
                        return result;
                return result;
            }
        }
        internal async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            _httpMethod = httpMethod;
            using (var request = await SetupHttpRequestMessage(this))
            {
                var result = await Send(request, cancellationToken);
                if (result.IsSuccessStatusCode)
                    if (_outputEventDuty != Interaction.Dontinteract)
                        return _outputEvent.Invoke(result);
                    else
                        return result;
                return result;
            }
        }
        internal async Task<HttpRequestMessage> SetupHttpRequestMessage(HORS hORS)
        {
            using (var output = new HttpRequestMessage(hORS._httpMethod, hORS._uri))
            {
                foreach (var item in output.Headers)
                {
                    if (!output.Headers.TryAddWithoutValidation(item.Key, item.Value))
                        throw new Exception($"{nameof(item.Key)} with the value of {item.Value} failed to add ");
                }
                output.Content = _httpContent;
                if (_inputEventDuty != Interaction.Dontinteract)
                    return await Task.Run(() => _inputEvent.Invoke(output));
                return output;
            }
        }
        #endregion 
        #region Get
        public async Task<HttpResponseMessage> GetAsync()
        {
            using (var result = await SendAsync(HttpMethod.Get))
            {
                return result;
            }
        }
        public async Task<U> GetDeserializeAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Get))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> GetDeserializeRawAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Get))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> GetBodyAsync()
        {
            using (var result = await SendAsync(HttpMethod.Get))
            {
                return result.Content;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Get, cancellationToken))
            {
                return result;
            }
        }
        public async Task<U> GetDeserializeAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Get, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> GetDeserializeRawAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Get, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> GetBodyAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Get, cancellationToken))
            {
                return result.Content;
            }
        }
        #endregion
        #region Delete
        public async Task<HttpResponseMessage> DeleteAsync()
        {
            using (var result = await SendAsync(HttpMethod.Delete))
            {
                return result;
            }
        }
        public async Task<U> DeleteDeserializeAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Delete))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> DeleteDeserializeRawAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Delete))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> DeleteBodyAsync()
        {
            using (var result = await SendAsync(HttpMethod.Delete))
            {
                return result.Content;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Delete, cancellationToken))
            {
                return result;
            }
        }
        public async Task<U> DeleteDeserializeAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Delete, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> DeleteDeserializeRawAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Delete, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> DeleteBodyAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Delete, cancellationToken))
            {
                return result.Content;
            }
        }
        #endregion
        #region Put      
        public async Task<HttpResponseMessage> PutAsync()
        {
            using (var result = await SendAsync(HttpMethod.Put))
            {
                return result;
            }
        }
        public async Task<U> PutDeserializeAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Put))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> PutDeserializeRawAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Put))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> PutBodyAsync()
        {
            using (var result = await SendAsync(HttpMethod.Put))
            {
                return result.Content;
            }
        }
        public async Task<HttpResponseMessage> PutAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Put, cancellationToken))
            {
                return result;
            }
        }
        public async Task<U> PutDeserializeAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Put, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> PutDeserializeRawAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Put, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> PutBodyAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Put, cancellationToken))
            {
                return result.Content;
            }
        }
        #endregion
        #region Post      
        public async Task<HttpResponseMessage> PostAsync()
        {
            using (var result = await SendAsync(HttpMethod.Post))
            {
                return result;
            }
        }
        public async Task<U> PostDeserializeAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Post))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> PostDeserializeRawAsync<U>()
        {
            using (var result = await SendAsync(HttpMethod.Post))
            {
                if (!_deserializerOutput.Equals(null))
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result);
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<HttpContent> PostBodyAsync()
        {
            using (var result = await SendAsync(HttpMethod.Post))
            {
                return result.Content;
            }
        }
        public async Task<HttpResponseMessage> PostAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Post, cancellationToken))
            {
                return result;
            }
        }
        public async Task<U> PostDeserializeAsync<U>(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Post, cancellationToken))
            {
                if (!_deserializerOutput.Equals(null))
                    return await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync());
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }
        public async Task<(U, HttpResponseMessage)> PostDeserializeRawAsync<U>(CancellationToken cancellationToken)
        {            
            using (var result = await SendAsync(HttpMethod.Post, cancellationToken))
            {                
                if (!_deserializerOutput.Equals(null))                    
                    return (await _deserializerOutput.Deserialize<U>(await result.Content.ReadAsStreamAsync()), result); 
                throw new NullReferenceException(nameof(_deserializerOutput));
            }
        }           
        public async Task<HttpContent> PostBodyAsync(CancellationToken cancellationToken)
        {
            using (var result = await SendAsync(HttpMethod.Post, cancellationToken))
            {
                return result.Content;
            }
        }

        public void Dispose()
        {           
            base.Dispose();
            _httpHeaders = null;
            if(_httpContent != null)
            _httpContent.Dispose();
            _httpMethod = null;
            _serializerInput = null;
            _serializerInput = null;
            _deserializerOutput = null;
            _inputEvent = null;
            _outputEvent = null;
            _inputEventDuty = Interaction.Default;
            _outputEventDuty = Interaction.Default;
        }
   
        #endregion
        #endregion
    }
    
}

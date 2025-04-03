using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace APIHandler
{
    public static class API
    {       
        public static void Init()
        {
            APIRunTimeParameters.SetParameters(); 
        }

        public static async Task<APIResponse> Get(string endpoint, List<APIParameter> parameters = null, List<APIHeader> headers = null, bool readAaBytes = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Response
                    HttpResponseMessage response = await client.GetAsync(endpoint);
                    if (!readAaBytes)
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method Get() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //Post string
        public static async Task<APIResponse> Post(string endpoint, string body, List<APIParameter> parameters = null, List<APIHeader> headers = null, Encoding? encoding = null, string? stringMediaType = "application/json", bool readAaBytes = false)
        {
            try
            {
                //Check
                encoding = encoding == null ? Encoding.UTF8 : encoding;

                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content
                    HttpContent content = new StringContent(body, encoding, stringMediaType);
                    //Response
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    if(!readAaBytes)
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method PostString() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //PostBinary
        public static async Task<APIResponse> Post(string endpoint, byte[] body, string contentType, List<APIParameter> parameters = null, List<APIHeader> headers = null, bool readAaBytes = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content                    
                    HttpContent content = new ByteArrayContent(body);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    //Response
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    if (!readAaBytes)
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method PostBinary() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //PostFormUrlEncodedContent
        public static async Task<APIResponse> PostForm(string endpoint, IEnumerable<KeyValuePair<string, string>> body, List<APIParameter> parameters = null, List<APIHeader> headers = null, bool readAaBytes = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content
                    FormUrlEncodedContent content = new FormUrlEncodedContent(body);
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    //Repsonse
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);
                    if (!readAaBytes)
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method PostFormUrlEncodedContent() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //PutString
        public static async Task<APIResponse> Put(string endpoint, string body, List<APIParameter> parameters = null, List<APIHeader> headers = null, Encoding? encoding = null, string? stringMediaType = "application/json", bool readAaBytes = false)
        {
            try
            {
                //Check
                encoding = encoding == null ? Encoding.UTF8 : encoding;

                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content
                    HttpContent content = new StringContent(body, encoding, stringMediaType);                    
                    //Response
                    HttpResponseMessage response = await client.PutAsync(endpoint, content);
                    //return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    if (!readAaBytes)
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                    return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method PutString() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //PutBinary
        public static async Task<APIResponse> Put(string endpoint, byte[] body, string contentType, List<APIParameter> parameters = null, List<APIHeader> headers = null, bool readAaBytes = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content                    
                    HttpContent content = new ByteArrayContent(body);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    //Response
                    HttpResponseMessage response = await client.PutAsync(endpoint, content);
                    //return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    if (!readAaBytes)
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method PutBinary() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }

        //Patch
        public static async Task<APIResponse> Patch(string endpoint, string body, List<APIParameter> parameters = null, List<APIHeader> headers = null, Encoding? encoding = null, string? stringMediaType = "application/json", bool readAaBytes = false)
        {
            try
            {
                //Check
                encoding = encoding == null ? Encoding.UTF8 : encoding;

                HttpClient client = new HttpClient();
                using (client)
                {
                    //Parameters
                    if (parameters != null && parameters.Count > 0)
                    {
                        StringBuilder stringParameters = new StringBuilder();
                        stringParameters.Append("?");
                        foreach (APIParameter parameter in parameters)
                            stringParameters.Append($"{parameter.Name}={parameter.Value}&");
                        endpoint += stringParameters.ToString();
                    }
                    //Headers
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (APIHeader header in headers)
                            client.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    //Content
                    HttpContent content = new StringContent(body, encoding, stringMediaType);
                    //Response
                    HttpResponseMessage response = await client.PatchAsync(endpoint, content);
                    if (!readAaBytes)
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, JsonResponse = response.Content.ReadAsStringAsync().Result };
                    else
                        return new APIResponse() { StatusCode = (int)response.StatusCode, Success = response.IsSuccessStatusCode, ByteResponse = response.Content.ReadAsByteArrayAsync().Result };
                }
            }
            catch (Exception exception)
            {
                APILogger.Write($"Method Patch() in API.cs", exception.Message, true);
                return new APIResponse() { Success = false, JsonResponse = string.Empty };
            }
        }
    }
}

[:arrow_up: ASC3](/PROJECTS/ASC3/ASC3.md)

# APIHandler
Module that simplifies communication with RESTful APIs. It uses HttpClient class from System.Net library to make requests.

## SOURCE CODE FILES
:link: [API.cs](/PROJECTS/ASC3/SOURCES/API.cs)\
:link: [APIClasses.cs](/PROJECTS/ASC3/SOURCES/APIClasses.cs)\
:link: [APIExampleUse.cs](/PROJECTS/ASC3/SOURCES/APIExampleUse.cs)\
:link: [APILogger.cs](/PROJECTS/ASC3/SOURCES/APILogger.cs)\
:link: [APIRuntimeParameters.cs](/PROJECTS/ASC3/SOURCES/APIRuntimeParameters.cs)

# Classes
**APIHandler** uses following classes:
``` csharp
public class APIResponse
{
    public int StatusCode { get; set; }
    public string JsonResponse { get; set; }
    public bool? Success { get; set; }
    public byte[] ByteResponse { get; set; }
}

public class APIParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class APIHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}
```
###### Code @ APIClasses.cs

# APIHandler Methods
All methods are included in [API.cs](/PROJECTS/ASC3/SOURCES/API.cs) file.\
Below, I present GET and POST method with example use with Allegro and Amazon API.
## GET
``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

public static class API
{       
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
}
```
###### Code @ API.cs (fragment)
Example use: Get Amazon orders
``` csharp
//GET
private async Task<AmazonOrder> GetOrder(string _server,string _orderId, string _token)
{
    //Http Headers
    APIHeader token = new APIHeader() { Name = "x-amz-access-token", Value = _token };

    List<APIHeader> _headers = new List<APIHeader>();
    _headers.Add(token);

    //Response
    APIResponse response = await API.Get($"{_server}/orders/v0/orders/{_orderId}/", headers: _headers);
    if (response.Success == false)
        return null;

    AmazonOrder order = JsonConvert.DeserializeObject<AmazonOrder>(response.JsonResponse);
    return order;
}
```
###### Code @ APIExampleUse.cs (fragment)

## POST
``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

public static class API
{       
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
}
```
###### Code @ API.cs (fragment)

Example use: Post image data to Allego.
``` csharp
//POST
private async Task<APIResponse> SendImage(string server, byte[] imageBytes)
{
    //Header
    APIHeader token = new APIHeader() { Name = "Authorization", Value = $"Bearer {_token}" };
    APIHeader accept = new APIHeader() { Name = "Accept", Value = "application/vnd.allegro.public.v1+json" };

    List<APIHeader> _headers = new List<APIHeader>();
    _headers.Add(token);
    _headers.Add(accept);

    //Response
    APIResponse response = await API.Post($"{server}/sale/images", imageBytes, "image/jpeg", headers: _headers);
    return response;
}
```
###### Code @ APIExampleUse.cs (fragment)

# APILogger
If **APIHandler** encounters exception it writes log to file with help of **APILogger** class:
``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class APILogger
{
    static bool copyToConsole = APIRunTimeParameters.CopyLogToConsole;
    public static void Write(string message, string description = "", bool error = false, string customType = "")
    {
        if (!APIRunTimeParameters.UseLogger) { return; }

        string path = APIRunTimeParameters.LogPath;
        StringBuilder information = new StringBuilder();

        information.Append($"{DateTime.Now} | ");
        if (customType == "")
        {
            if (error)
            {
                information.Append("ERROR | ");
            }
            else
            {
                information.Append("INFO  | ");
            }
        }
        else
        {
            information.Append($"{customType} | ");
        }
        information.Append(message);
        if (description != "")
        {
            information.Append($" : {description}");
        }
        information.Append("\n");

        File.AppendAllText(path, information.ToString());
        if (copyToConsole) Console.WriteLine(information.ToString());
    }
}
```
###### Code @ APILogger.cs
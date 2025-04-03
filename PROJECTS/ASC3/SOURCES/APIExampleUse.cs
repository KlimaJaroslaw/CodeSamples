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
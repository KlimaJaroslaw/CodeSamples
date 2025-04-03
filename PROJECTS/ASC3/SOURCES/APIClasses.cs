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
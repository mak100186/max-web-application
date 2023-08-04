namespace Maxx.TestCommon.EndpointMock;
public class Mocks<TRequest, TResponse>
{
    private string _target;
    public string Target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
            var elements = _target.Split(".");
            Client = elements[0];
            Method = elements[1];
        }
    }

    public string Client { get; set; }
    public string Method { get; set; }
    public List<Mock<TRequest, TResponse>> MockData { get; set; }
}

public class Mock<TRequest, TResponse>
{
    public Dictionary<string, TRequest> Request { get; set; } = new();
    public TResponse Response { get; set; }
}











namespace RandomNumberCore;

[ServiceContract]
public interface IStreamingService
{
    [OperationContract]
    Stream GetRandomStream();
}
    

sealed class StreamingService: IStreamingService
{
    readonly IHttpContextAccessor context;
    public StreamingService( IHttpContextAccessor context ) =>
        this.context = context;

    public Stream GetRandomStream()
    {
        CancellationToken token = context.HttpContext?.RequestAborted ?? CancellationToken.None;
        return new RandomStream( Random.Shared, token );
    }
}
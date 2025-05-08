using System.ServiceModel;

namespace RandomNumberConsumerNet8
{
    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract]
        Stream GetRandomStream();
    }
    public interface IStreamingServiceChannel : IStreamingService, IClientChannel;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            using var channelFactory = new ChannelFactory<IStreamingServiceChannel>(new BasicHttpBinding(BasicHttpSecurityMode.Transport){TransferMode = TransferMode.Streamed, MaxReceivedMessageSize = 1_000_000_000 }, new EndpointAddress("https://localhost:7151/StreamingService.svc"));
            using var service = channelFactory.CreateChannel();
            service.Open();
            using var randomStream = service.GetRandomStream();
            byte[] buffer = new byte[4];
            await randomStream.ReadExactlyAsync(buffer, cts.Token);
            service.Close();
            channelFactory.Close();
        }
    }
}
using System.Diagnostics;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.WebHost.UseKestrel(options =>
{
    options.AllowSynchronousIO = true;
    options.ListenLocalhost(7151, listenOptions =>
    {
        listenOptions.UseHttps();
        if (Debugger.IsAttached)
        {
            listenOptions.UseConnectionLogging();
        }
    });
});

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<StreamingService>();
    serviceBuilder.AddServiceEndpoint<StreamingService, IStreamingService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport){TransferMode = TransferMode.Streamed}, $"https://localhost:7151/StreamingService.svc");
 
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();

using System.Net;

namespace SlotBookingApp.Tests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    public Func<HttpRequestMessage, HttpResponseMessage> SendAsyncFunc { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (SendAsyncFunc != null)
        {
            return Task.FromResult(SendAsyncFunc(request));
        }
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}

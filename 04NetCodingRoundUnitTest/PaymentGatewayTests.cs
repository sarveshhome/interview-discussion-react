using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

public class PaymentGatewayTests
{
    private readonly Mock<IConfiguration> _config = new();
    private readonly Mock<ILogger<PaymentGateway>> _logger = new();

    private PaymentGateway CreateSut(HttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://api.test/") }, _config.Object, _logger.Object);

    private static Mock<HttpMessageHandler> SetupHandler(HttpStatusCode status, object? content = null)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(status)
            {
                Content = content != null ? JsonContent.Create(content) : new StringContent(string.Empty)
            });
        return handler;
    }

    // --- GetQuoteAsync ---

    [Fact]
    public async Task GetQuoteAsync_ReturnsQuote_WhenSuccess()
    {
        var expected = new PaymentQuote { Fee = 1.5m, Currency = "USD" };
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK, expected).Object);

        var result = await sut.GetQuoteAsync("cust1", CancellationToken.None);

        Assert.Equal(expected.Fee, result.Fee);
        Assert.Equal(expected.Currency, result.Currency);
    }

    [Fact]
    public async Task GetQuoteAsync_ThrowsArgumentException_WhenCustomerIdEmpty()
    {
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK).Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.GetQuoteAsync("", CancellationToken.None));
    }

    [Fact]
    public async Task GetQuoteAsync_ThrowsArgumentException_WhenCustomerIdWhitespace()
    {
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK).Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.GetQuoteAsync("   ", CancellationToken.None));
    }

    [Fact]
    public async Task GetQuoteAsync_ThrowsHttpRequestException_WhenApiFails()
    {
        var sut = CreateSut(SetupHandler(HttpStatusCode.BadGateway).Object);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            sut.GetQuoteAsync("cust1", CancellationToken.None));
    }

    [Fact]
    public async Task GetQuoteAsync_ThrowsInvalidOperationException_WhenResponseIsNull()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
            });
        var sut = CreateSut(handler.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GetQuoteAsync("cust1", CancellationToken.None));
    }

    // --- CreatePaymentAsync ---

    [Fact]
    public async Task CreatePaymentAsync_ReturnsResult_WhenSuccess()
    {
        _config.Setup(c => c["PaymentGateway:ApiKey"]).Returns("test-key");
        var expected = new PaymentResult { PaymentId = "pay_1", Status = "Success" };
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK, expected).Object);
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 100m, Currency = "USD" };

        var result = await sut.CreatePaymentAsync(request, CancellationToken.None);

        Assert.Equal(expected.PaymentId, result.PaymentId);
        Assert.Equal(expected.Status, result.Status);
    }

    [Fact]
    public async Task CreatePaymentAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK).Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.CreatePaymentAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task CreatePaymentAsync_ThrowsInvalidOperationException_WhenApiKeyMissing()
    {
        _config.Setup(c => c["PaymentGateway:ApiKey"]).Returns((string?)null);
        var sut = CreateSut(SetupHandler(HttpStatusCode.OK).Object);
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 100m, Currency = "USD" };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreatePaymentAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreatePaymentAsync_ThrowsHttpRequestException_WhenApiFails()
    {
        _config.Setup(c => c["PaymentGateway:ApiKey"]).Returns("test-key");
        var sut = CreateSut(SetupHandler(HttpStatusCode.InternalServerError).Object);
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 100m, Currency = "USD" };

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            sut.CreatePaymentAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreatePaymentAsync_ThrowsInvalidOperationException_WhenResponseIsNull()
    {
        _config.Setup(c => c["PaymentGateway:ApiKey"]).Returns("test-key");
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
            });
        var sut = CreateSut(handler.Object);
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 100m, Currency = "USD" };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreatePaymentAsync(request, CancellationToken.None));
    }
}

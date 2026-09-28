using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentGateway> _gateway = new();
    private readonly Mock<ILogger<PaymentsController>> _logger = new();
    private readonly PaymentsController _sut;

    public PaymentsControllerTests()
    {
        _sut = new PaymentsController(_gateway.Object, _logger.Object);
    }

    // --- GetQuote ---

    [Fact]
    public async Task GetQuote_ReturnsOk_WithQuote()
    {
        var quote = new PaymentQuote { Fee = 2.5m, Currency = "GBP" };
        _gateway.Setup(g => g.GetQuoteAsync("cust1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(quote);

        var result = await _sut.GetQuote("cust1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(quote, ok.Value);
    }

    [Fact]
    public async Task GetQuote_PropagatesException_WhenGatewayThrows()
    {
        _gateway.Setup(g => g.GetQuoteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _sut.GetQuote("cust1", CancellationToken.None));
    }

    // --- Create ---

    [Fact]
    public async Task Create_ReturnsOk_WithPaymentResult()
    {
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 50m, Currency = "USD" };
        var paymentResult = new PaymentResult { PaymentId = "pay_1", Status = "Success" };
        _gateway.Setup(g => g.CreatePaymentAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentResult);

        var result = await _sut.Create(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(paymentResult, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsValidationProblem_WhenModelStateInvalid()
    {
        _sut.ModelState.AddModelError("Amount", "Amount is required");
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 0m, Currency = "USD" };

        var result = await _sut.Create(request, CancellationToken.None);

        Assert.IsType<ObjectResult>(result.Result);
        _gateway.Verify(g => g.CreatePaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_PropagatesException_WhenGatewayThrows()
    {
        var request = new PaymentRequest { CustomerId = "cust1", Amount = 50m, Currency = "USD" };
        _gateway.Setup(g => g.CreatePaymentAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API error"));

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _sut.Create(request, CancellationToken.None));
    }
}

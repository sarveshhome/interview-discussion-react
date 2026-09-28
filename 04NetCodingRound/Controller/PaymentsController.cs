using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentGateway paymentGateway,
        ILogger<PaymentsController> logger)
    {
        _paymentGateway = paymentGateway;
        _logger = logger;
    }

    [HttpGet("quote/{customerId}")]
    public async Task<ActionResult<PaymentQuote>> GetQuote(
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await _paymentGateway.GetQuoteAsync(
            customerId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("payment")]
    public async Task<ActionResult<PaymentResult>> Create(
        [FromBody] PaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _paymentGateway.CreatePaymentAsync(
            request,
            cancellationToken);

        return Ok(result);
    }
}
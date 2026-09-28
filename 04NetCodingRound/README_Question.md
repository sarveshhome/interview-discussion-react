### ask by interviewer

```
services.AddScoped<IPaymentGateway, PaymentGateway>();

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentGateway _payments;

    public PaymentsController(IPaymentGateway payments)
    {
        _payments = payments;
    }

    [HttpGet("quote/{customerId}")]
    public async PaymentQuote GetQuote(string customerId)
    {
        return _payments.GetQuoteAsync(customerId).Result;
    }

    
    public async Task<PaymentResult> Create(PaymentRequest request)
    {
        try
        {
            return await _payments.CreatePaymentAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}

public class PaymentGateway : IPaymentGateway
{
    private const string BaseUrl = "https://api.payments.example.com/";
    private const string ApiKey = "sk_live_51HxYz...";

    public async Task<PaymentQuote> GetQuoteAsync(string customerId)
    {
        var client = new HttpClient();
        var json = await client.GetStringAsync(
            BaseUrl + "v1/quotes?customerId=" + customerId);
        return JsonSerializer.Deserialize<PaymentQuote>(json);
    }

    public async Task<PaymentResult> CreatePaymentAsync(PaymentRequest request)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);

        var response = await client.PostAsync(
            BaseUrl + "v1/payments",
            new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"));

        var body = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaymentResult>(body);
    }
}

public class PaymentRequest
{
    public string CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class PaymentQuote
{
    public decimal Fee { get; set; }
    public string Currency { get; set; }
}

public class PaymentResult
{
    public string PaymentId { get; set; }
    public string Status { get; set; }
}
}

```



### For local development, you can use User Secrets:

```
dotnet user-secrets set "PaymentGateway:ApiKey" "sk_live_xxxxxxxxx"
```
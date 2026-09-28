using System.Net.Http.Headers;
using System.Net.Http.Json;

public class PaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentGateway> _logger;

    public PaymentGateway(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<PaymentGateway> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PaymentQuote> GetQuoteAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException(
                "CustomerId is required.",
                nameof(customerId));
        }

        var url =
            $"v1/quotes?customerId={Uri.EscapeDataString(customerId)}";

        _logger.LogInformation(
            "Getting payment quote for customer {CustomerId}",
            customerId);

        using var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Payment quote API failed. StatusCode: {StatusCode}",
                response.StatusCode);

            throw new HttpRequestException(
                $"Payment quote API returned {response.StatusCode}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<PaymentQuote>(
                cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Payment quote response was empty.");
        }

        return result;
    }

    public async Task<PaymentResult> CreatePaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var apiKey =
            _configuration["PaymentGateway:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Payment gateway API key is not configured.");
        }

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "v1/payments");

        httpRequest.Headers.Add("X-Api-Key", apiKey);

        httpRequest.Content =
            JsonContent.Create(request);

        _logger.LogInformation(
            "Creating payment for customer {CustomerId}",
            request.CustomerId);

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            _logger.LogError(
                "Payment API failed. StatusCode: {StatusCode}, Response: {Response}",
                response.StatusCode,
                errorBody);

            throw new HttpRequestException(
                $"Payment API returned {response.StatusCode}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<PaymentResult>(
                cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Payment response was empty.");
        }

        return result;
    }
}
public interface IPaymentGateway
{
    Task<PaymentQuote> GetQuoteAsync(
        string customerId,
        CancellationToken cancellationToken);

    Task<PaymentResult> CreatePaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken);
}
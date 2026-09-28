using System.ComponentModel.DataAnnotations;

public class PaymentRequest
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;
}
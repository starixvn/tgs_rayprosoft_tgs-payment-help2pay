using System.Xml.Serialization;

namespace Tgs.Payment.Help2Pay;

[XmlRoot(ElementName = "MerchantService.CheckStatus", Namespace = "http://schemas.datacontract.org/2004/07/Services.Public")]
public record QueryPaymentWithdrawFormResponse
{
    [XmlElement(ElementName = "Amount")]
    public decimal Amount { get; init; }

    [XmlElement(ElementName = "CurrencyCode")]
    public string CurrencyCode { get; init; } = default!;

    [XmlElement(ElementName = "CustomerID")]
    public string PlayerCode { get; init; } = default!;

    [XmlElement(ElementName = "ID")]
    public string WithdrawId { get; init; } = default!;

    [XmlElement(ElementName = "Key")]
    public string Key { get; init; } = default!;

    [XmlElement(ElementName = "StatusCode")]
    public string StatusCode { get; init; } = default!;

    [XmlElement(ElementName = "Message")]
    public string? Message { get; init; }

    [XmlElement(ElementName = "UpdatedDate")]
    public string UpdatedDate { get; init; } = default!;
}

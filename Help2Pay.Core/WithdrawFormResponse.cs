using System.Xml.Serialization;

namespace Tgs.Payment.Help2Pay;

[XmlRoot(ElementName = "Payout")]
public record WithdrawFormResponse
{
    [XmlElement(ElementName = "statusCode")]
    public string StatusCode { get; init; } = default!;

    [XmlElement(ElementName = "message")]
    public string Message { get; init; } = default!;
}

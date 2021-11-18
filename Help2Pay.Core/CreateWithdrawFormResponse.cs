using System.Xml.Serialization;

namespace Tgs.Payment.Help2Pay;

[XmlRoot(ElementName = "Payout")]
public record CreateWithdrawFormResponse
{
    [XmlElement(ElementName = "statusCode")]
    public string StatusCode { get; init; }

    [XmlElement(ElementName = "message")]
    public string Message { get; init; }
}

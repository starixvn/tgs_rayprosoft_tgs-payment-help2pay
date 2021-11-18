using System.Xml.Serialization;

namespace Help2PayTest;

[XmlRoot(ElementName = "Payout")]
public record WithdrawResponse
{
    [XmlElement(ElementName = "statusCode")]
    public string StatusCode { get; init; }

    [XmlElement(ElementName = "message")]
    public string Message { get; init; }
}

using System.Security.Cryptography;
using System.Text;

namespace Tgs.Payment.Help2Pay
{
    public class Help2PayHasher
    {
        public string ComputeHash(string securityCode, IDictionary<string, object> parameters)
        {
            var merchant = parameters.ContainsKey("Merchant") ? $"{parameters["Merchant"]}" : string.Empty;
            var reference = parameters.ContainsKey("Reference") ? $"{parameters["Reference"]}" : string.Empty;
            var customer = parameters.ContainsKey("Customer") ? $"{parameters["Customer"]}" : string.Empty;
            var amount = parameters.ContainsKey("Amount") ? $"{(decimal)parameters["Amount"]:f2}" : string.Empty;
            var currency = parameters.ContainsKey("Currency") ? $"{parameters["Currency"]}" : string.Empty;
            var datetime = parameters.ContainsKey("DateTime") ? $"{(DateTimeOffset)parameters["DateTime"]:yyyyMMddHHmmss}" : string.Empty;
            var clientIp = parameters.ContainsKey("ClientIP") ? $"{parameters["ClientIP"]}" : string.Empty;

            var data = $"{merchant}{reference}{customer}{amount}{currency}{datetime}{securityCode}{clientIp}";
            var hash = string.Join(
                string.Empty,
                MD5.HashData(Encoding.UTF8.GetBytes(data)).Select(b => b.ToString("x2")));

            return hash;
        }
    }
}
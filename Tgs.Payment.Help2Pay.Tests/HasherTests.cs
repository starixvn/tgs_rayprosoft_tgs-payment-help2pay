using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Security.Cryptography;

namespace Tgs.Payment.Help2Pay.Tests;

[TestFixture]
public class HasherTests
{
    [Test]
    public void Should_Match_Help2Pay_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "PHP";
        var customer = "customer";
        var referenceId = Guid.NewGuid();
        var now = DateTimeOffset.Now;
        var amount = 71m;
        var ip = "127.0.0.1";

        var dict = new Dictionary<string, object>
        {
            ["Merchant"] = merchant,
            ["Currency"] = currency,
            ["Customer"] = customer,
            ["Reference"] = referenceId,
            ["Amount"] = amount,
            ["DateTime"] = now,
            ["FrontURI"] = "http://www.google.com",
            ["BackURI"] = "http://www.google.com",
            ["Bank"] = "BDO",
            ["Language"] = "en-us",
            ["ClientIP"] = ip
        };

        var expected = HashCode(securityCode, merchant, customer, referenceId, currency, now, amount, ip);

        var actual = sut.ComputeHash(securityCode, dict);

        Assert.AreEqual(expected, actual);
    }

    private string HashCode(
        string securityCode,
        string merchant,
        string customer,
        Guid referenceId,
        string currecy,
        DateTimeOffset now,
        decimal amount,
        string ip)
    {
        var data = $"{merchant}{referenceId}{customer}{amount:f2}{currecy}{now:yyyyMMddHHmmss}{securityCode}{ip}";

        return string.Join(
                string.Empty,
                MD5.HashData(Encoding.UTF8.GetBytes(data)).Select(b => b.ToString("x2")));
    }
}
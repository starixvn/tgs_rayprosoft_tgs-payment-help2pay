using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace Tgs.Payment.Help2Pay.Tests;

[TestFixture]
public class HasherTests
{
    [Test]
    public void Should_Match_Help2Pay_Deposit_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "PHP";
        var customer = "customer";
        var referenceId = Guid.NewGuid();
        var now = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(8)).DateTime;
        var amount = 71m;
        var ip = "127.0.0.1";

        var info = new DepositHasherInfo(customer, currency, amount, referenceId, now);

        var data = $"{merchant}{referenceId}{customer}{amount:f2}{currency}{now:yyyyMMddHHmmss}{securityCode}{ip}";
        var expected = HashCode(data);

        var actual = sut.ComputeHash(merchant, securityCode, info, ip);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Should_Match_Help2Pay_Deposit_Confirm_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "THB";
        var customer = "help2pay_2";
        var referenceId = Guid.Parse("a89d15f7-ab3d-44a5-af05-27d340d317fe");
        var amount = 50.00m;
        var status = "000";

        var info = new DepositConfirmHasherInfo(customer, currency, amount, referenceId, status);

        var expected = "4C668A2380FECD279695CE3E14DCD2DF";

        var actual = sut.ComputeHash(merchant, securityCode, info);

        Assert.True(expected.Equals(actual, StringComparison.OrdinalIgnoreCase));
    }

    private string HashCode(string original)
        => string.Join(
                string.Empty,
                MD5.HashData(Encoding.UTF8.GetBytes(original)).Select(b => b.ToString("x2")));
}
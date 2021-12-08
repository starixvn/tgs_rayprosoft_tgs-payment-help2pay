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
    public void Should_Match_Help2Pay_Deposit_Confirm_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "THB";
        var referenceId = Guid.NewGuid();
        var customer = referenceId.ToString();
        var amount = 50.00m;
        var status = "000";

        var info = new DepositConfirmHasherInfo(customer, currency, amount, referenceId, status);

        var data = $"{merchant}{referenceId}{customer}{amount:f2}{currency}{status}{securityCode}";
        var expected = HashCode(data);

        var actual = sut.ComputeHash(merchant, securityCode, info);

        Assert.True(expected.Equals(actual, StringComparison.OrdinalIgnoreCase));
    }

    [Test]
    public void Should_Match_Help2Pay_Deposit_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "PHP";
        var referenceId = Guid.NewGuid();
        var customer = referenceId.ToString();
        var now = DateTime.UtcNow;
        var amount = 71m;
        var ip = "127.0.0.1";

        var info = new DepositHasherInfo(customer, currency, amount, referenceId, now);

        var timeConverted = new DateTimeOffset(now).ToOffset(TimeSpan.FromHours(8D)).DateTime.ToString("yyyyMMddHHmmss");
        var data = $"{merchant}{referenceId}{customer}{amount:f2}{currency}{timeConverted}{securityCode}{ip}";
        var expected = HashCode(data);

        var actual = sut.ComputeHash(merchant, securityCode, info, ip);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Should_Match_Help2Pay_Withdraw_Confirm_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "THB";
        var transactionId = Guid.NewGuid();
        var memberCode = transactionId.ToString();

        var amount = 50.00m;
        var status = "000";

        var info = new WithdrawConfirmHasherInfo(memberCode, currency, amount, transactionId, status);

        var data = $"{merchant}{transactionId}{memberCode}{amount:f2}{currency}{status}{securityCode}";
        var expected = HashCode(data);

        var actual = sut.ComputeHash(merchant, securityCode, info);

        Assert.True(expected.Equals(actual, StringComparison.OrdinalIgnoreCase));
    }

    [Test]
    public void Should_Match_Help2Pay_Withdraw_Hash_Rule()
    {
        var sut = new Help2PayHasher();

        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var merchant = "INT0492";
        var currency = "PHP";
        var transactionId = Guid.NewGuid();
        var memberCode = transactionId.ToString();
        var now = DateTime.UtcNow;
        var amount = 71m;
        var bankAccount = "bank_account";

        var info = new WithdrawHasherInfo
        {
            Amount = amount,
            TransactionId = transactionId,
            MemberCode = memberCode,
            CurrencyCode = currency,
            BankAccount = bankAccount,
            TransactionTime = now
        };

        var timeConverted = new DateTimeOffset(now).ToOffset(TimeSpan.FromHours(8D)).DateTime.ToString("yyyyMMddHHmmss");
        var data = $"{merchant}{transactionId}{memberCode}{amount:f2}{currency}{timeConverted}{bankAccount}{securityCode}";
        var expected = HashCode(data);

        var actual = sut.ComputeHash(merchant, securityCode, info);

        Assert.AreEqual(expected, actual);
    }

    private string HashCode(string original)
        => string.Join(
                string.Empty,
                MD5.HashData(Encoding.UTF8.GetBytes(original)).Select(b => b.ToString("x2")));
}

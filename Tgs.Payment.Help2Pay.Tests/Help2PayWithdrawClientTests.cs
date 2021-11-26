using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Tgs.Payment.Help2Pay.Tests;

[TestFixture]
public class Help2PayWithdrawClientTests
{
    [Test]
    public async Task Should_Cache_Hash()
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(@"<Payout><statusCode>000</statusCode><message>Success</message></Payout>"),
        };

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var mockHasher = new Mock<Help2PayHasher>();
        var mockWithdrawStore = new Mock<IWithdrawDataStore>();
        var logger = NullLogger<Help2PayWithdrawClient>.Instance;

        var merchantId = "INT0492";
        var securityCode = "juJFlG7Jo3yrdeK1Dmeg";

        var order = new WithdrawOrder
        {
            MemberCode = "playerCode",
            Amount = 100m,
            Currency = "PHP",
            BankCode = "BDO",
            TransactionId = Guid.NewGuid(),
            TransactionTime = DateTime.UtcNow,
            CallbackUrl = $"http://www.google.com",
            BankAccount = "0012212345697",
            BankAccountName = "steve",
            ClientIp = "127.0.0.1"
        };

        var sut = new Help2PayWithdrawClient(httpClient, mockHasher.Object, mockWithdrawStore.Object, logger);

        _ = await sut.WithdrawAsync(merchantId, securityCode, order).ConfigureAwait(false);

        mockWithdrawStore.Verify(fake
           =>
           fake.StoreHashAsync(It.IsAny<string>(), It.Is<Guid>(x => x == order.TransactionId)),
           Times.Exactly(1));
    }
}

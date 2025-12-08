using System.Net;
using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.Integration.Endpoints;

[TestFixture]
public class RoomEndpointsIntegrationTests
{
    private WebApplicationFactory<TestEntryPoint> factory;
    private IRoomManager roomManagerFake;

    [SetUp]
    public void Setup()
    {
        roomManagerFake = A.Fake<IRoomManager>();
        
        factory = new WebApplicationFactory<TestEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(roomManagerFake);
                });
            });
    }

    [TearDown]
    public void TearDown()
    {
        factory.Dispose();
    }

    [Test]
    public async Task GetRoomPrivacy_WhenRoomExists_ReturnsPrivacyInfo()
    {
        var roomId = Guid.NewGuid();
        const RoomPrivacy expectedPrivacy = RoomPrivacy.Public;
        
        A.CallTo(() => roomManagerFake.GetRoomPrivacyAsync(roomId, A<CancellationToken>._))
            .Returns(OperationResult<RoomPrivacy>.Ok(expectedPrivacy));

        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/rooms/{roomId}");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.ToLower(), Does.Contain("public"));
        });

        A.CallTo(() => roomManagerFake.GetRoomPrivacyAsync(roomId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
using System.Net;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.Integration.Endpoints;

[TestFixture]
public class RoomEndpointsIntegrationTests
{
    private WebApplicationFactory<TestEntryPoint> factory;
    private IRoomManager roomManagerFake;
    private IUserRepository userRepositoryFake;

    [SetUp]
    public void Setup()
    {
        roomManagerFake = A.Fake<IRoomManager>();
        userRepositoryFake = A.Fake<IUserRepository>();
        
        factory = new WebApplicationFactory<TestEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(roomManagerFake);
                    services.AddSingleton(userRepositoryFake);
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

    [Test]
    public async Task GetRoomPrivacy_WhenRoomDoesNotExist_ReturnsNotFound()
    {
        var roomId = Guid.NewGuid();
        
        A.CallTo(() => roomManagerFake.GetRoomPrivacyAsync(roomId, A<CancellationToken>._))
            .Returns(OperationResult<RoomPrivacy>.Error.NotFound("не нашел"));

        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/rooms/{roomId}");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content.ToLower(), Does.Contain("не нашел"));
        });

        A.CallTo(() => roomManagerFake.GetRoomPrivacyAsync(roomId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task GetAvailableRooms_WhenRoomExists_ReturnsAvailableRooms()
    {
        var room1 = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        var room2 = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);
        var room3 = new Room(Guid.NewGuid(), RoomPrivacy.Public, 10);

        A.CallTo(() => roomManagerFake.GetAvailablePublicRoomsAsync(A<CancellationToken>._))
            .Returns(OperationResult<IEnumerable<Room>>.Ok([room1, room2, room3]));
    
        using var client = factory.CreateClient();
    
        var response = await client.GetAsync("/api/rooms");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null.Or.Empty);
        });
        Assert.That(content.Trim(), Does.StartWith("["));
        Assert.Multiple(() =>
        {
            Assert.That(content.Trim(), Does.EndWith("]"));
            Assert.That(content, Does.Match(@"\""id\"":\s*\""[a-fA-F0-9\-]+\"""));
        });

        A.CallTo(() => roomManagerFake.GetAvailablePublicRoomsAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    public async Task GetAvailableRooms_WhenRoomNotExists()
    {
        A.CallTo(() => roomManagerFake.GetAvailablePublicRoomsAsync(A<CancellationToken>._))
            .Returns(OperationResult<IEnumerable<Room>>.Error.NotFound());
    
        using var client = factory.CreateClient();
    
        var response = await client.GetAsync("/api/rooms");
       
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            
        A.CallTo(() => roomManagerFake.GetAvailablePublicRoomsAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
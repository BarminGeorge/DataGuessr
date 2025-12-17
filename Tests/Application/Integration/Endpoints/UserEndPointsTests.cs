using System.Net;
using System.Text;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.Integration.Endpoints;

[TestFixture]
public class UserEndpointsTests
{
    private WebApplicationFactory<TestEntryPoint> factory;
    private IUserService userServiceFake;

    [SetUp]
    public void Setup()
    {
        userServiceFake = A.Fake<IUserService>();
        var roomManagerFake = A.Fake<IRoomManager>();
    
        factory = new WebApplicationFactory<TestEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(userServiceFake);
                    services.AddSingleton(roomManagerFake);
                });
            });
    }

    [TearDown]
    public void TearDown() => factory.Dispose();
    
    [Test]
    public async Task Register_ReturnsSuccess()
    {
        var user = new User("Player1", new Avatar("filename", "mimeType"));
        A.CallTo(() => userServiceFake.Register(A<string>._, A<string>._,  
                A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(OperationResult<User>.Ok(user));

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Testuser"), "Login" },
            { new StringContent("Password123"), "Password" },
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/register", formData);

        response.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task Register_ReturnsInternalError()
    {
        A.CallTo(() => userServiceFake.Register(A<string>._, A<string>._,  
                A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(OperationResult<User>.Error.InternalError());

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Testuser"), "Login" },
            { new StringContent("Password123"), "Password" },
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/register", formData);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
    
    [TestCase("", "password123", "Player1", Description = "Empty login")]
    [TestCase("testuser", "", "Player1", Description = "Empty password")]
    [TestCase("testuser", "password123", "", Description = "Empty player name")]
    [TestCase("a", "password123", "Player1", Description = "Login too short")]
    [TestCase("testuser", "123", "Player1", Description = "Password too short")]
    [TestCase("verylongusernameexceedingmaxlimitverylongusernameexceedingmaxlimitverylongusernameexceedingmaxlimitverylongusernameexceedingmaxlimit", "password123", "Player1", Description = "Login too long")]
    public async Task Register_InvalidParameter_ReturnsBadRequest(string login, string password, string playerName)
    {
        using var client = factory.CreateClient();
        
        var formData = new MultipartFormDataContent
        {
            { new StringContent(login), "Login" },
            { new StringContent(password), "Password" },
            { new StringContent(playerName), "PlayerName" }
        };
        
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/register", formData);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task UpdateUser_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        
        A.CallTo(() => userServiceFake.UpdateUser(A<Guid>._, A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(new OperationResult<string>(true, ""));

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            {new  StringContent(userId.ToString()), "UserId" },
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/userUpdate", formData);

        response.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task UpdateUser_ReturnsInternalError()
    {
        var userId = Guid.NewGuid();
        
        A.CallTo(() => userServiceFake.UpdateUser(A<Guid>._, A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(OperationResult<string>.Error.InternalError());

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            {new  StringContent(userId.ToString()), "UserId" },
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/userUpdate", formData);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
    
    [Test]
    public async Task CreateGuest_ReturnsSuccess()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("playerName", avatar);
        A.CallTo(() => userServiceFake.CreateGuest(A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(OperationResult<User>.Ok(user));

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/guest", formData);

        response.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task CreateGuest_ReturnsInternalError()
    {
        A.CallTo(() => userServiceFake.CreateGuest(A<string>._, A<IFormFile>._, A<CancellationToken>._))
            .Returns(OperationResult<User>.Error.InternalError());

        using var client = factory.CreateClient();
    
        var fakeImageBytes = new byte[] { 
            0xFF, 0xD8, 0xFF, 0xE0, // JPEG сигнатура
            0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 // JFIF
        };
    
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Player1"), "PlayerName" }
        };
    
        var fileContent = new ByteArrayContent(fakeImageBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent, "Avatar", "test.jpg");

        var response = await client.PostAsync("/api/guest", formData);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
    
    [Test]
    public async Task Login_ReturnsInternalError()
    {
        A.CallTo(() => userServiceFake.Login(A<string>._, A<string>._, A<CancellationToken>._))
            .Returns(OperationResult<(string, User)>.Error.InternalError());

        using var client = factory.CreateClient();
    
        var loginRequest = new { Login = "Testuser", Password = "testPass22" };
        var jsonContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");
        
        var response = await client.PostAsync("/api/login", jsonContent);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }
    
    [Test]
    public async Task Login_ReturnsSuccess()
    {
        var result = ("avnk.aivna.l", new User("test", new Avatar("filename", "mimeType")));
        A.CallTo(() => userServiceFake.Login(A<string>._, A<string>._, A<CancellationToken>._))
            .Returns(OperationResult<(string, User)>.Ok(result));

        using var client = factory.CreateClient();
    
        var loginRequest = new { Login = "Testuser", Password = "testPass22" };
        var jsonContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");
        
        var response = await client.PostAsync("/api/login", jsonContent);

        response.EnsureSuccessStatusCode();
    }
}
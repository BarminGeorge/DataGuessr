using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FakeItEasy;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Tests.Application.Implementations;

[TestFixture]
public class UserServiceTests
{
    protected IAvatarRepository AvatarRepository;
    protected IUserRepository UserRepository;
    protected UserService UserService;
    protected IFormFile FormFile;
    
    protected CancellationToken Ct = CancellationToken.None;
    private IJwtProvider jwtProvider;
    protected IPasswordHasher PasswordHasher;
    
    [SetUp]
    public void SetUp()
    {
        AvatarRepository = A.Fake<IAvatarRepository>();
        UserRepository = A.Fake<IUserRepository>();
        jwtProvider = A.Fake<IJwtProvider>();
        FormFile = A.Fake<IFormFile>();
        PasswordHasher = A.Fake<IPasswordHasher>();
        UserService = new UserService(jwtProvider, UserRepository, AvatarRepository, PasswordHasher);
    }
}

public class RegisterTests : UserServiceTests
{
    [Test]
    public async Task SuccessRegisterTest()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .Returns(OperationResult.Ok());
        
        var result = await UserService.Register("BarminG", "veryStrongPassword228", "gb", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task Register_WhenCanNotSaveAvatar()
    {
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Error.ExternalServiceError());

        var result = await UserService.Register("BarminG", "veryStrongPassword228", "gb", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith("Ошибка внешнего сервиса"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ExternalServiceError));
        });
    }
    
    [Test]
    public async Task Register_WhenCanNotSaveAvatar_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.ExternalServiceError(),
                OperationResult<Avatar>.Ok(avatar));
        
        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .Returns(OperationResult.Ok());

        var result = await UserService.Register("BarminG", "veryStrongPassword228", "gb", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task Register_WhenCanNotAddUserInRepository()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .Returns(OperationResult.Error.AlreadyExists());
        
        var result = await UserService.Register("BarminG", "veryStrongPassword228", "gb", FormFile, Ct);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith("Ресурс уже существует"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.AlreadyExists));
        });
    }
    
    [Test]
    public async Task Register_WhenAddUserInRepository_ThirdSecondAttempt()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult.Error.AlreadyExists(),
                OperationResult.Error.AlreadyExists(),
                OperationResult.Ok());
        
        var result = await UserService.Register("BarminG", "veryStrongPassword228", "gb", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
}

public class LoginTests : UserServiceTests
{
    [Test]
    public async Task SuccessLoginTest()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");
        
        A.CallTo(() => UserRepository.GetByLoginAsync("BarminG", Ct))
            .Returns(OperationResult<User>.Ok(user));

        A.CallTo(() => PasswordHasher.Verify("veryStrongPassword228", "password"))
            .Returns(true);
        
        var result = await UserService.Login("BarminG", "veryStrongPassword228",  Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj.user, Is.EqualTo(user));
        });
    }
    
    [Test]
    public async Task SuccessLoginTest_WhenGetUserReturnError_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");
        
        A.CallTo(() => UserRepository.GetByLoginAsync("BarminG", Ct))
            .ReturnsNextFromSequence(
                OperationResult<User>.Error.InternalError(),
                OperationResult<User>.Error.ExternalServiceError(),
                OperationResult<User>.Ok(user));

        A.CallTo(() => PasswordHasher.Verify("veryStrongPassword228", "password"))
            .Returns(true);
        
        var result = await UserService.Login("BarminG", "veryStrongPassword228",  Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj.user, Is.EqualTo(user));
        });
    }
    
    [Test]
    public async Task Login_WhenGetUserReturnError_3_Times()
    {
        A.CallTo(() => UserRepository.GetByLoginAsync("BarminG", Ct))
            .ReturnsNextFromSequence(
                OperationResult<User>.Error.ExternalServiceError(),
                OperationResult<User>.Error.ExternalServiceError(),
                OperationResult<User>.Error.InternalError());
        
        var result = await UserService.Login("BarminG", "veryStrongPassword228",  Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.InternalError));
        });
    }
    
    [Test]
    public async Task Login_WhenInvalidPassword()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");
        
        A.CallTo(() => UserRepository.GetByLoginAsync("BarminG", Ct))
            .Returns(OperationResult<User>.Ok(user));

        A.CallTo(() => PasswordHasher.Verify("veryStrongPassword228", "password"))
            .Returns(false);
        
        var result = await UserService.Login("BarminG", "veryStrongPassword228",  Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Unauthorized));
            Assert.That(result.ErrorMessage, Does.Contain("Invalid username or password"));
        });
    }
}

public class UpdateUserTests : UserServiceTests
{
    [Test]
    public async Task SuccessUpdateUserTest()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));
        
        A.CallTo(() => UserRepository.UpdateUserAsync(user.Id, avatar, "newName", Ct))
            .Returns(OperationResult.Ok());
        
        var result = await UserService.UpdateUser(user.Id, "newName", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task SuccessUpdateUserTest_WhenGetUserReturnError_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.ExternalServiceError(),
                OperationResult<Avatar>.Ok(avatar));
        
        A.CallTo(() => UserRepository.UpdateUserAsync(user.Id, avatar, "newName", Ct))
            .Returns(OperationResult.Ok());
        
        var result = await UserService.UpdateUser(user.Id, "newName", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task UpdateUserTest_WhenGetUserReturnError_3_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.InternalError());
        
        var result = await UserService.UpdateUser(user.Id, "newName", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.InternalError));
        });
    }
    
    [Test]
    public async Task UpdateUserTest_WhenCanNotUpdateUser_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));
        
        A.CallTo(() => UserRepository.UpdateUserAsync(user.Id, avatar, "newName", Ct))
            .ReturnsNextFromSequence(
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError(),
                OperationResult.Ok());
        
        var result = await UserService.UpdateUser(user.Id, "newName", FormFile, Ct);
        
        Assert.That(result.Success, Is.True);
    }
    
    [Test]
    public async Task UpdateUserTest_WhenCanNotUpdateUser_3_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        var user = new User("BarminG", "veryStrongPassword228", avatar, "password");

        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));
        
        A.CallTo(() => UserRepository.UpdateUserAsync(user.Id, avatar, "newName", Ct))
            .ReturnsNextFromSequence(
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError());
        
        var result = await UserService.UpdateUser(user.Id, "newName", FormFile, Ct);
        
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorType, Is.EqualTo(ErrorType.InternalError));
    }
}

public class CreateGuestTests : UserServiceTests
{
    [Test]
    public async Task SuccessCreateUserTest_WhenCreateUserReturnError_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .Returns(OperationResult.Ok());
        
        var result = await UserService.CreateGuest("gb", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj.IsGuest, Is.True);
        });
    }
    
    [Test]
    public async Task SuccessCreateUserTest_WhenCreateUserReturnError_3_Times()
    {
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.InternalError(),
                OperationResult<Avatar>.Error.InternalError());
        
        var result = await UserService.CreateGuest("gb", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.InternalError));
        });
    }
    
    [Test]
    public async Task SuccessCreateUserTest_WhenAddUserReturnError_2_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError(),
                OperationResult.Ok());
        
        var result = await UserService.CreateGuest("gb", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultObj.IsGuest, Is.True);
        });
    }
    
    [Test]
    public async Task SuccessCreateUserTest_WhenAddUserReturnError_3_Times()
    {
        var avatar = new Avatar("filename", "mimetype");
        
        A.CallTo(() => AvatarRepository.SaveUserAvatarAsync(A<IFormFile>._, Ct))
            .Returns(OperationResult<Avatar>.Ok(avatar));

        A.CallTo(() => UserRepository.AddAsync(A<User>._, Ct))
            .ReturnsNextFromSequence(
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError(),
                OperationResult.Error.InternalError());
        
        var result = await UserService.CreateGuest("gb", FormFile, Ct);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.InternalError));
        });
    }
}
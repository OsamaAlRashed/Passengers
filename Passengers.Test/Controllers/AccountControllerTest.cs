using Neptunee.xApi;
using Passengers.Controllers;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Xunit.Abstractions;

namespace Passengers.Test.Controllers;

public class AccountControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;


    public AccountControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
        _fixture.AdminAuth();
    }

    [Fact]
    public async Task GivenValidUserNameAndPassword_WhenLogin_ThenSuccess()
    {
        await _fixture.Api.Rout<AccountController>(nameof(AccountController.Login))
            .FromBody(new BaseLoginDto
            {
                UserName = "SuperUser",
                Password = "1111",
            }).SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task GivenValidInfo_WhenSignUpAdmin_ThenSuccess()
    {
        await _fixture.Api.Rout<AccountController>(nameof(AccountController.SignUp))
            .FromBody(new CreateAccountDto
            {
                UserName = $"testUser{Guid.NewGuid()}",
                PhoneNumber = "09999999",
                Type = SharedKernel.Enums.UserType.Customer,
                Password = "1111"
            }).SendAsync()
            .AssertSuccessStatusCodeAsync();
    }
}

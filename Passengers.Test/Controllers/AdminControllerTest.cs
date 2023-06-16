using Neptunee.xApi;
using Passengers.Controllers;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.Models.Security;
using Xunit.Abstractions;

namespace Passengers.Test.Controllers;

public class AdminControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public AdminControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
        _fixture.AdminAuth();
    }

    [Fact]
    public async Task WhenCallGet_ThenReturnAllAdminsAndSuccess()
    {
        await _fixture.Api.Rout<AdminController>(nameof(AdminController.Get))
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task WhenCallGetById_ThenSuccess()
    {
        await _fixture.Api.Rout<AdminController>(nameof(AdminController.GetById))
            .FromQuery("id", _fixture.GetId<AppUser>())
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Test1()
    {
        await _fixture.Api.Rout<CustomerController>(nameof(CustomerController.SignUp))
            .FromBody(new CreateAccountCustomerDto
            {
                FullName = _fixture.Faker.Name.FullName(),
                DOB = _fixture.Faker.Date.Between(new DateTime(2000, 1, 1), new DateTime(2000, 1, 1)),
                Gender = SharedKernel.Enums.GenderType.Male,
                Password = "1111",
                PhoneNumber = "0999"
            })
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }
}

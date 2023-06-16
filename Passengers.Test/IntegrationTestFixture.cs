using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neptunee.xApi;
using Passengers.Controllers;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SqlServer.DataBase;

namespace Passengers.Test;

public class IntegrationTestFixture : IntegrationTest<Startup>
{
    private readonly PassengersDbContext _dbcontext;

    public IntegrationTestFixture()
    {
        Configure(webApplicationBuilder: builder => builder.UseEnvironment(Environments.Development),
            clientBuilder: options => { options.AllowAutoRedirect = false; });

        var serviceScope = Api.ApplicationFactory.Services.CreateScope();
        _dbcontext = serviceScope.ServiceProvider.GetRequiredService<PassengersDbContext>();
    }

    public Guid GetId<T>()
        where T : class, IBaseEntity
        => _dbcontext.Set<T>().First().Id;

    public Guid GetUserId(UserType userType)
        => _dbcontext.Set<AppUser>().Where(x => x.UserType == userType).First().Id;

    public void AdminAuth()
    {
        Api.JwtAuthenticate(Api.Rout<AccountController>(nameof(AccountController.Login))
            .FromBody(new BaseLoginDto
            {
                UserName = "SuperUser",
                Password = "1111",
            }).Send()
            .Content
            .ReadAs<LoginResponseDto>()
            .AccessToken);
    }

    public void CustomerAuth()
    {
        Api.JwtAuthenticate(Api.Rout<CustomerController>(nameof(CustomerController.Login))
            .FromBody(new LoginCustomerDto
            {
                PhoneNumber = "0956057886",
                Password = "1111",
            }).Send()
            .Content
            .ReadAs<object>()
            .GetValueReflection<object, string>("accessToken"));
    }
}

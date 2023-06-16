using Neptunee.xApi;
using Passengers.Controllers;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Models.Location;
using Passengers.Models.Main;
using Xunit.Abstractions;

namespace Passengers.Test.Controllers;

public class OrderControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;


    public OrderControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
        _fixture.CustomerAuth();
    }

    [Fact]
    public async Task GivenValidUserNameAndPassword_WhenLogin_ThenSuccess1()
    {
        await _fixture.Api.Rout<OrderController>(nameof(OrderController.AddOrder))
            .FromBody(new SetOrderDto
            {
                AddressId = _fixture.GetId<Address>(),
                DriverNote = "Note",
                Cart = new List<ResponseCardDto>()
                {
                    new ResponseCardDto
                    {
                        Id = _fixture.GetUserId(SharedKernel.Enums.UserType.Shop),
                        Note = "",
                        Products = new List<ProductCardDto>()
                        {
                            new ProductCardDto
                            {
                                Id = _fixture.GetId<Product>(),
                                Count = 1,
                            }
                        }
                    }
                } 
            }).SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

}

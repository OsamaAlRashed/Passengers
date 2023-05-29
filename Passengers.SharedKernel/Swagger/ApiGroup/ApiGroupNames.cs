namespace Passengers.SharedKernel.Swagger.ApiGroup;

public enum ApiGroupNames
{
    [GroupInfo(Title = "All", Description = "all", Version = "v1")]
    All = 0,
    [GroupInfo(Title = "Dashboard", Description = "dashboard", Version = "v1")]
    Dashboard = 1,
    [GroupInfo(Title = "Shop", Description = "Shop", Version = "v1")]
    Shop = 2,
    [GroupInfo(Title = "Customer", Description = "Customer", Version = "v1")]
    Customer = 3,
    [GroupInfo(Title = "Driver", Description = "Driver", Version = "v1")]
    Driver = 4,
    [GroupInfo(Title = "Test", Description = "test", Version = "v1")]
    Test = 5,
}

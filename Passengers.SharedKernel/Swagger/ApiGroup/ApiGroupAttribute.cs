namespace Passengers.SharedKernel.Swagger.ApiGroup;

public class ApiGroupAttribute : System.Attribute
{
    public ApiGroupAttribute(params ApiGroupNames[] name)
    {
        GroupName = name;
    }
    public ApiGroupNames[] GroupName { get; set; }
}

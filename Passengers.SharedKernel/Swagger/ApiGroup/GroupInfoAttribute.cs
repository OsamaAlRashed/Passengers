namespace Passengers.SharedKernel.Swagger.ApiGroup;

public class GroupInfoAttribute : System.Attribute
{
    public string Title { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
}

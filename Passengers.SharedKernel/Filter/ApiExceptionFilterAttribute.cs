using Passengers.SharedKernel.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Passengers.SharedKernel.Filter;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new JsonResult(context.Exception.ToFullException()) { StatusCode = 500 };
        base.OnException(context);
    }
}

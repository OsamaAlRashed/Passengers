using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.OperationResult.ExtensionMethods
{
    public static class OperationExtensions
    {
        public static OperationResult<T> ToOperationResult<T>(this T @object)
         => new OperationResult<T>().SetSuccess(@object);

        public static OperationResult<T> WithStatusCode<T>(this OperationResult<T> result, int statusCode)
        {
            result.StatusCode = statusCode;
            return result;
        }

        public static JsonResult ToJsonResult<T>(this OperationResult<T> result)
        {
            return result.OperationResultType switch
            {
                OperationResultTypes.Success => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status200OK), null, true),
                OperationResultTypes.Exist => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status202Accepted), result.Message),
                OperationResultTypes.NotExist => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status404NotFound), result.Message),
                OperationResultTypes.Failed => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status400BadRequest), result.Message),
                OperationResultTypes.Forbidden => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status403Forbidden), result.Message),
                OperationResultTypes.Unauthorized => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status401Unauthorized), result.Message),
                OperationResultTypes.Exception => result.GetValidResult(result.HasCustomeStatusCode.NestedIF<int>(result.StatusCode ?? 0, StatusCodes.Status500InternalServerError), result.FullExceptionMessage),
                _ => new JsonResult(string.Empty),
            };
        }

        public static async Task<JsonResult> ToJsonResultAsync<T>(this Task<OperationResult<T>> result) => (await result).ToJsonResult();

        private static JsonResult GetValidResult<T>(this OperationResult<T> result, int statusCode, string jsonMessage = null, bool json = false) =>
            json ? new JsonResult(result.Result) { StatusCode = statusCode } :
                   new JsonResult(jsonMessage.IsNullOrEmpty() ? result.OperationResultType.ToString() : jsonMessage) { StatusCode = statusCode };

    }
}

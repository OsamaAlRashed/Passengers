using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.SharedKernel.OperationResult.Enums
{
    public enum OperationResultTypes
    {
        Success,
        Exist,
        NotExist,
        Failed,
        Forbidden,
        Exception,
        Unauthorized
    }
}

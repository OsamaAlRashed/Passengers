using Passengers.SharedKernel.OperationResult.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.SharedKernel.OperationResult.Base
{
    public class OperationResultBase : IEquatable<OperationResultBase>, IDisposable
    {
        public string Message { get; set; }

        public OperationResultTypes OperationResultType { get; set; }

        public Exception Exception { get; set; }

        public int? StatusCode { get; set; }

        public bool Equals(OperationResultBase other)
        {
            return Message == other.Message && OperationResultType == other.OperationResultType &&
                (Exception == other.Exception || Exception.Message == other.Exception.Message) && StatusCode == other.StatusCode;
        }

        bool disposed = false;

        ~OperationResultBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Exception = null;
                Message = null;
            }

            disposed = true;
        }

    }
}

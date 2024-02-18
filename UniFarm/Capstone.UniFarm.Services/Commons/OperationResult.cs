using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.Commons
{
    public class OperationResult<T>
    {
        public T? Payload { get; set; }
        public bool IsError { get; set; }
        public string? AccessToken { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
        public void AddError(ErrorCode code, string message)
        {
            HandleError(code, message);
        }
        public void AddUnknownError(string message)
        {
            HandleError(ErrorCode.UnknownError, message);
        }

        public void ResetIsErrorFlag()
        {
            IsError = false;
        }

        private void HandleError(ErrorCode code, string message)
        {
            Errors.Add(new Error { Code = code, Message = message });
            IsError = true;
        }

        public void AddValidationError(string foodIdAndSupplierIdCannotBeTheSame)
        {
            HandleError(ErrorCode.UnknownError, foodIdAndSupplierIdCannotBeTheSame);
        }
    }
}

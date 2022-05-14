using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class GenericAPIResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

        public GenericAPIResponse()
        { }

        public GenericAPIResponse(string responseCode, string responseDescription)
        {
            ResponseCode = responseCode;
            ResponseDescription = responseDescription;
        }
    }

    public class GenericAPIResponse<T> : GenericAPIResponse
    {
        public T Data { get; set; }

        public GenericAPIResponse(string responseCode, string responseDescription, T data)
            : base(responseCode, responseDescription)
        {
            Data = data;
        }

        public GenericAPIResponse()
        { }
    }
}

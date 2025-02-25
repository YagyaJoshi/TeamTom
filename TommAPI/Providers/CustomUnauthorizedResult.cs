using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TommAPI.Provider
{
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message,int Code)
            : base(new CustomError(message,Code))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
            Code = StatusCodes.Status401Unauthorized;
        }
    }
}
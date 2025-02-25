using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace TommAPI.Providers
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Handle the exception and create an error response.
            var result = new ObjectResult(new
            {
                Success = false,
                Message = context.Exception.Message
            })
            {
                StatusCode = 500, // You can set an appropriate status code for the error.
            };

            context.Result = result;
            Console.WriteLine(context.Exception.Message);
        }
    }
}

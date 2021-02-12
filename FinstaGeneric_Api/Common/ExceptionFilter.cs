using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FinstaApi.Common
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<ExceptionFilter> _Logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _Logger = logger;           
        }        
        public override void OnException(ExceptionContext context)
        {
            Log.Information(" ");
            Log.Information("---------- Exception Start Here-----------");
            Log.Error(context.Exception.ToString()); // Logs Exceptions into Filesystem
        }


        //public void OnException(ExceptionContext context)
        //{
        //    HttpStatusCode status = HttpStatusCode.InternalServerError;
        //    String message = String.Empty;

        //    var exceptionType = context.Exception.GetType();
        //    if (exceptionType == typeof(UnauthorizedAccessException))
        //    {
        //        message = "Unauthorized Access";
        //        status = HttpStatusCode.Unauthorized;
        //    }
        //    else if (exceptionType == typeof(NotImplementedException))
        //    {
        //        message = "A server error occurred.";
        //        status = HttpStatusCode.NotImplemented;
        //    }
        //    else if (exceptionType == typeof(FinstaAppException))
        //    {
        //        message = context.Exception.ToString();
        //        status = HttpStatusCode.InternalServerError;
        //    }
        //    else
        //    {
        //        message = context.Exception.Message;
        //        status = HttpStatusCode.NotFound;
        //    }
        //    context.ExceptionHandled = true;
        //    HttpResponse response = context.HttpContext.Response;
        //    response.StatusCode = (int)status;
        //    response.ContentType = "application/json";
        //    var err = message + " " + context.Exception.StackTrace;
        //    Log.Information(" ");
        //    Log.Information("---------- Exception Start Here-----------");
        //    Log.Logger.Error(err); // Logs Exceptions into Filesystem
        //    response.WriteAsync(err);           
        //}       
    }
}

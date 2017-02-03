using CommonLib.Dashboard.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Api.Filters
{
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public UnhandledExceptionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public override void OnException(ExceptionContext context)
        {
            var errorId = Guid.NewGuid().ToString();

            _logger.LogError($"WebApi controller exception: {errorId}", context.Exception, errorId);

            context.HttpContext.Response.Headers.Add("aconex-error-id", errorId);

            // string errorMessage;
            if (context.Exception is NotImplementedException)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotImplemented);
            }
            //else if (context.Exception is EntityNotFoundException)
            //{
            //    context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
            //}
            //else if (context.Exception is InputException)
            //{
            //    context.Result = new BadRequestObjectResult(ErrorResponse.CreateFromInputException((InputException)context.Exception));
            //}
            //else if (context.Exception is AlreadyExistsException)
            //{
            //    context.Result = new ConflictObjectResult(ErrorResponse.CreateFromAlreadyExistsException((AlreadyExistsException)context.Exception));
            //}
            //else if (TryExceptionOrFaultOfType<ValidationException>(context.Exception, out errorMessage))
            //{
            //    context.Result = new BadRequestObjectResult(errorMessage);
            //}
            else
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            if (context.HttpContext.Request.Path.StartsWithSegments("/api")) return;
            context.Result = null;
        }

        private bool TryExceptionOrFaultOfType<T>(Exception ex, out string errorMessage)
        {
            errorMessage = null;
            if (ex is T)
            {
                errorMessage = ex.Message;
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Seemplest.Core.ServiceObjects;
using Seemplest.Core.Tracing;
using SeemplestBlocks.Core.Diagnostics;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WebApiExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            // --- Log this information to the trace log
            var logItem = new TraceLogItem
            {
                Type = TraceLogItemType.Error,
                OperationType = "WebAPI",
                Message = "Exception",
                DetailedMessage = context.Exception.ToString()
            };
            Tracer.Log(logItem);

            var businessEx = context.Exception as BusinessOperationException;
            if (businessEx != null)
            {
                // --- This is a business issue
                var info = new BusinessExceptionInfo
                {
                    reasonCode = businessEx.ReasonCode,
                    isBusiness = true,
                    message = businessEx.Message,
                    errorObject = businessEx.Notifications.Items
                };
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(info))
                });
            }

            // --- This is an infrastructure issue
            var infraInfo = new InfrastructureExceptionInfo
            {
                reasonCode = "Unexpected",
                isBusiness = false,
                message = context.Exception.Message
            };
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(infraInfo))
            });
        }

        // ReSharper disable InconsistentNaming
        public class InfrastructureExceptionInfo
        {
            public string reasonCode { get; set; }
            public bool isBusiness { get; set; }
            public string message { get; set; }
        }

        public class BusinessExceptionInfo : InfrastructureExceptionInfo
        {
            public object errorObject { get; set; }
        }
        // ReSharper restore InconsistentNaming
    }
}
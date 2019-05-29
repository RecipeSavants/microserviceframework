using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using RecipeSavants.Microservices.Filters.Models;

namespace RecipeSavants.Microservices.Filters
{
    public class StopwatchFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var d = DateTime.Parse(context.HttpContext.Request.Headers["Elapased-Start"]);
            var d1 = DateTime.UtcNow;
            var d2 = context.ActionDescriptor.DisplayName + "-Elapased " + d1;
            var stopwatch = new Stopwatch()
            {
                Action = context.ActionDescriptor.DisplayName,
                Route = JsonConvert.SerializeObject(context.ActionDescriptor.RouteValues),
                StartTime = d,
                EndTime = d1,
                ElapsedTime = d1 - d1
            };
            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.Add("Elapased-Start", DateTime.UtcNow.ToString());
            base.OnActionExecuting(context);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInMins;

        public RedisCacheAttribute(int durationInMins = 5)
        {
            _durationInMins = durationInMins;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var cacheService =
                context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = CreateCacheKey(context.HttpContext.Request);
            var cacheValue = await cacheService.GetAsync(cacheKey);

            if (cacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = cacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };

                return;
            }

            var ExecutedContext = await next.Invoke();

            if (ExecutedContext.Result is OkObjectResult result)
            {
                await cacheService.SetAsync(
                    cacheKey,
                    result.Value!,
                    TimeSpan.FromMinutes(_durationInMins)
                );
            }
        }

        
        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();

            key.Append(request.Path); 

            foreach (var item in request.Query.OrderBy(X => X.Key))
            {
                key.Append($"|{item.Key}-{item.Value}");
            }

            return key.ToString();
        }
    }
}

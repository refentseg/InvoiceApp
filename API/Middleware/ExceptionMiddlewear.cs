using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddlewear
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddlewear> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddlewear(RequestDelegate next,ILogger<ExceptionMiddlewear> logger,IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex){
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType="application/json";
                context.Response.StatusCode= 500;

                var response = new ProblemDetails
                {
                    Status = 500,
                    Detail = _env.IsDevelopment() ? ex.StackTrace?.ToString() : null,
                    Title = ex.Message
                };

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response,options);
                await context.Response.WriteAsync(json);

            }
        }
    }
}
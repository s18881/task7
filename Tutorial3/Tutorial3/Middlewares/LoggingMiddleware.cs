using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tutorial3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next; 
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            string bodyOfRequest;
            string queryString;
            /*if (File.Exists(@"Logs/requestsLog.txt"))
            {
                File.Delete(@"Logs/requestsLog.txt");
            }
            else
            {
                File.Create(@"Logs/requestsLog.txt");
            }*/
            
            using (var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyOfRequest = await streamReader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;
            }
            
            queryString = httpContext.Request.QueryString.ToString();
            if (String.IsNullOrEmpty(queryString))
            {
                queryString = "\"Empty\"";
            }

            await using (var streamWriter = new StreamWriter(@"Logs/requestsLog.txt", true))
                streamWriter.WriteLine("1. HTTP Method (" + httpContext.Request.Method + ")\n" +
                                       "2. Endpoint Path (" + httpContext.Request.Path + ")\n" +
                                       "3. Body of each request (" + bodyOfRequest + ")\n" +
                                       "4. Query strings (" + queryString + ")\n\n");
            await _next(httpContext); 
        }
    }
    
}
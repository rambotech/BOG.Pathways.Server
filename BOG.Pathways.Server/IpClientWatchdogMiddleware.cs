using BOG.Pathways.Server.Models;
using BOG.Pathways.Server.StorageModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server
{
    public class IpClientWatchdogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MemoryStorage _storage;

        private object LockPoint = new object();

        public IpClientWatchdogMiddleware(RequestDelegate next, MemoryStorage storage)
        {
            _next = next;
            _storage = storage;
        }

        public Task Invoke(HttpContext httpContext)
        {
            lock (LockPoint)
            {
                string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (!_storage.IpWatchList.ContainsKey(ipAddress))
                {
                    _storage.IpWatchList.Add(ipAddress, new IpWatch()
                    {
                        IpAddress = ipAddress,
                        IsWhitelisted = false,
                        LatestAttempt = DateTime.Now
                    });
                }
                var ipEntry = _storage.IpWatchList[ipAddress];
                if (ipEntry.FailedAttempts >= 3 && DateTime.Now.Subtract(ipEntry.LatestAttempt).TotalMinutes < 5)
                {
                    httpContext.Response.StatusCode = 451;
                    var buff = System.Text.Encoding.UTF8.GetBytes("You are not playing nice.");
                    httpContext.Response.Body.Write(buff, 0, buff.Length);
                    return Task.CompletedTask;
                }
                else
                {
                    return _next(httpContext);
                }
            }
        }
    }

    public static class IpClientWatchdogMiddlewareExtensions
    {
        public static IApplicationBuilder UseIpClientWatchdogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpClientWatchdogMiddleware>();
        }
    }
}

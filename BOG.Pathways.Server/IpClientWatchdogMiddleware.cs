using BOG.Pathways.Common.Dto;
using BOG.Pathways.Server.Interface;
using BOG.Pathways.Server.Models;
using BOG.Pathways.Server.StorageModels;
using BOG.Pathways.Server.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.Entity;

namespace BOG.Pathways.Server
{
    /// <summary>
    /// Middleware to block client IP addresses with repeated credential failures.
    /// </summary>
    public class IpClientWatchdogMiddleware
    {
        private readonly RequestDelegate _next;

        private object LockPoint = new object();

        /// <summary>
        /// Instantiation via injection
        /// </summary>
        /// <param name="next">the next middleware</param>
        public IpClientWatchdogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// The work to do.
        /// </summary>
        /// <param name="httpContext">the http information</param>
        /// <param name="storage">the storage for the serer operation</param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext, IStorage storage)
        {
            lock (LockPoint)
            {
                string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
                if (!storage.IpWatchList.ContainsKey(ipAddress))
                {
                    storage.IpWatchList.Add(ipAddress, new IpWatch()
                    {
                        IpAddress = ipAddress,
                        IsWhitelisted = false,
                        LatestAttempt = DateTime.Now
                    });
                }
                var ipEntry = storage.IpWatchList[ipAddress];
                if (ipEntry.FailedAttempts >= 3 && DateTime.Now.Subtract(ipEntry.LatestAttempt).TotalMinutes < 1)
                {
                    httpContext.Response.StatusCode = 451;
                    var buff = System.Text.Encoding.UTF8.GetBytes(Serializer<ErrorResponse>.ToJson(ErrorResponseManifest.Get(-1)));
                    httpContext.Response.Body.Write(buff, 0, buff.Length);
                    ipEntry.LatestAttempt = DateTime.Now;
                    return Task.CompletedTask;
                }
                else
                {
                    if (ipEntry.FailedAttempts >= 3)
                    {
                        ipEntry.FailedAttempts--;
                    }
                    ipEntry.LatestAttempt = DateTime.Now;
                    return _next(httpContext);
                }
            }
        }
    }

    /// <summary>
    /// Extensions for building the middleware
    /// </summary>
    public static class IpClientWatchdogMiddlewareExtensions
    {
        /// <summary>
        /// Add the middle ware to the middleware sequence.
        /// </summary>
        /// <param name="builder">from startup</param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpClientWatchdogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpClientWatchdogMiddleware>();
        }
    }
}

﻿using BOG.Pathways.Server.Models;
using BOG.Pathways.Server.StorageModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOG.Pathways.Server
{
    /// <summary>
    /// Middleware to block client IP addresses with repeated credential failures.
    /// </summary>
    public class IpClientWatchdogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MemoryStorage _storage;

        private object LockPoint = new object();

        /// <summary>
        /// Instantiation via injection
        /// </summary>
        /// <param name="next">the next middleware</param>
        /// <param name="storage">the storage for settings/pathways/cleints</param>
        public IpClientWatchdogMiddleware(RequestDelegate next, MemoryStorage storage)
        {
            _next = next;
            _storage = storage;
        }

        /// <summary>
        /// The work to do.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
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
        public static IApplicationBuilder UseIpClientWatchdogMiddleware(this IApplicationBuilder builder, MemoryStorage memoryStorage)
        {
            return builder.UseMiddleware<IpClientWatchdogMiddleware>(memoryStorage);
        }
    }
}

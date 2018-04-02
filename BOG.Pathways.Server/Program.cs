using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.Helpers;
using BOG.Pathways.Server.Interface;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace BOG.Pathways.Server
{
    public class Program
    {
        static readonly SimpleInjector.Container container;

        /// <summary>
        /// Main start class
        /// </summary>
        static Program()
        {
        }

        /// <summary>
        /// Main start point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .Run();
        }

        /// <summary>
        /// Main start point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                }).Build();
    }
}
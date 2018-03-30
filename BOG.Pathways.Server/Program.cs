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

        static Program()
        {
            container = new Container();

            container.RegisterSingleton<IStorage, MemoryStorage>();
            container.RegisterSingleton<Security>();

            container.Verify();
        }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}

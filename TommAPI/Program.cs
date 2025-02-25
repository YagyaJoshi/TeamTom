using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;   
using Microsoft.Extensions.Logging;

namespace TommAPI
{
    public class Program
    {
        /* It is Start The application*/
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        /* This method is connect the application to web*/
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

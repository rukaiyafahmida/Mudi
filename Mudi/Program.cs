using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mudi
{
    public class Program
    {
        static Program()
        {
            if (OperatingSystem.IsLinux())
            {
                // Runtime-only Ubuntu installations expose libsqlite3.so.0;
                // the unversioned libsqlite3.so name requires a development package.
                var provider = Assembly.Load("SQLitePCLRaw.provider.sqlite3");
                NativeLibrary.SetDllImportResolver(provider, (name, assembly, searchPath) =>
                    name == "sqlite3" ? NativeLibrary.Load("libsqlite3.so.0", assembly, searchPath) : IntPtr.Zero);
            }
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

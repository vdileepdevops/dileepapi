using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinstaApi.Security.Hashing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Finsta_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {        
            FileInfo fileInfo = new FileInfo("appsettings.json");
            var ExceptionLogpath = fileInfo.DirectoryName + "\\" + "Finsta_Exception_Logs";
            if (!Directory.Exists(ExceptionLogpath))
            {
                Directory.CreateDirectory(ExceptionLogpath);
            }
            Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.RollingFile(Path.Combine(ExceptionLogpath, "Finsta_Exception_Log-{Date}.txt"))
           .CreateLogger();
            try
            {                
                Log.Information("Application starting");
                var host = BuildWebHost(args);
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var passwordHasher = services.GetService<IPasswordHasher>();
                }
                host.Run();
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            /*
             * The call to ".UseIISIntegration" is necessary to fix issue while running the API from ISS. See the following links for reference:
             * - https://github.com/aspnet/IISIntegration/issues/242
             * - https://stackoverflow.com/questions/50112665/newly-created-net-core-gives-http-400-using-windows-authentication
            */

            WebHost.CreateDefaultBuilder(args)
                   .UseIISIntegration()
                   .UseStartup<Startup>()
                   .Build();
    }
}

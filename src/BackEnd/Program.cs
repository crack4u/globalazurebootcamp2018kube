using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BackEnd
{
    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            #if DEBUG
                   .UseUrls("http://localhost:5001")
            #endif
                   .UseStartup<Startup>();
    }
}

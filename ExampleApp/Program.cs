using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetMQ.Controllers.Extensions;

namespace ExampleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder().UseConsoleLifetime();
            builder.UseNetMQControllers();
            var host = builder.Build();
            await host.StartAsync();
        }
    }
}
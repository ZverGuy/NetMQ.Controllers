using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Controllers.Core;
using NetMQ.Controllers.Extensions;
using NetMQ.Sockets;

namespace ExampleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder().UseConsoleLifetime();
            builder.ConfigureLogging(x => x.AddConsole());
            builder.UseNetMQControllers();
            var host = builder.Build();
            await host.StartAsync();

            using (DealerSocket socket = new DealerSocket())
            {
                var message = new NetMQMessage();
                message.AppendEmptyFrame();
                message.Append("ads");
                socket.Connect("inproc://test");
                socket.SendMultipartMessage(message);
            }

            Console.ReadKey();
        }
    }
}
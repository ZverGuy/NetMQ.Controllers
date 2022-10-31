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
            using var socket = new RequestSocket();
            var message = new NetMQMessage();
          
           

            var builder = new HostBuilder().UseConsoleLifetime();
            builder.ConfigureLogging(x => x.AddConsole());
            builder.UseNetMQControllers();
            var host = builder.Build();
            await host.StartAsync();
            socket.Connect("inproc://test");
            message.Append("lol");
            socket.SendMultipartMessage(message);
            Console.ReadKey();
        }
    }
}
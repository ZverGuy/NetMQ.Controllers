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
            using var socket = new PublisherSocket();
            var message = new NetMQMessage();
            socket.Bind("inproc://test");
           

            var builder = new HostBuilder().UseConsoleLifetime();
            builder.ConfigureLogging(x => x.AddConsole());
            builder.UseNetMQControllers();
            var host = builder.Build();
            await host.StartAsync();
            message.Append("TestTopic");
            message.Append("Lol");
            socket.SendMultipartMessage(message);
            Console.ReadKey();
        }
    }
}
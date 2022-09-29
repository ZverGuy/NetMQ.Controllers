using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetMQ.Controllers.Core;
using NetMQ.Controllers.Core.SocketFactories;
using NetMQ.Sockets;

namespace NetMQ.Controllers.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNetMQControllers(this IHostBuilder builder)
        {
            var controllers = ControllerHelper.GetControllers();
            var factories = ControllerHelper.GetSocketFactories();
            foreach (var controller in controllers)
            {
                builder = builder.ConfigureServices(((context, collection) => collection.AddSingleton(controller)));
            }

            foreach (var factory in factories)
            {
                builder = builder.ConfigureServices(((context, collection) => collection.AddSingleton(factory)));
            }
            
            builder = builder.ConfigureServices(
                (context, collection) => collection.AddSingleton<SocketFactory>());
            builder = builder.ConfigureServices(
                (context, collection) => collection.AddSingleton<SocketCollection>());
            
            return builder.ConfigureServices(
                (context, collection) => collection.AddHostedService<NetMQHostedService>());
            
        }
    }
}
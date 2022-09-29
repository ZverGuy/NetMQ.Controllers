using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetMQ.Controllers.Core.SocketFactories;
using NetMQ.Sockets;

namespace NetMQ.Controllers.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNetMQControllers(this IHostBuilder builder)
        {
            var controllers = ControllerHelper.GetControllers();
            foreach (var controller in controllers)
            {
                builder = builder.ConfigureServices(((context, collection) => collection.AddSingleton(controller)));
            }
            builder = builder.ConfigureServices(
                (context, collection) => collection.AddSingleton<SocketFactory>());
            return builder.ConfigureServices(
                (context, collection) => collection.AddHostedService<NetMQHostedService>());
            
        }
    }
}
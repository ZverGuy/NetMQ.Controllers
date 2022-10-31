using System;
using System.Threading.Tasks;
using NetMQ.Controllers;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Controllers.Core;
using NetMQ.Sockets;

namespace ExampleApp
{
    public class TestController
    {
        [RouterSocket(ConnectionString ="inproc://test")]
        [RegexMatch("lol", 2)]
        public async Task<int> LmaoTest(MessageContext<RouterSocket, string> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 2;
        }
    }
}
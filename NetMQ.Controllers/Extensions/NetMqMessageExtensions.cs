using System.Linq;
using System.Text;

namespace NetMQ.Controllers.Extensions
{
    public static class NetMqMessageExtensions
    {
        public static string ReadAllFramesAsString(this NetMQMessage message)
        {
            var sb = new StringBuilder();
            var frames = message.Skip(1);
            foreach (var frame in frames)
            {
                sb.Append(frame.ConvertToString());
            }
            return sb.ToString();
        }
    }
}
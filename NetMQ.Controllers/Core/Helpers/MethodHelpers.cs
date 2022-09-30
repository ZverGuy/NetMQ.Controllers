using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NetMQ.Controllers.Core;
using NetMQ.Controllers.Extensions;
using Newtonsoft.Json;

namespace NetMQ.Controllers
{
    internal static class MethodHelpers
    {
        /// <summary>
        /// Get what Context used in handler
        /// <see cref="NetMQMessageContext{TSocket}"/> or <see cref="MessageContext{TSocket,TMessage}"/>
        /// </summary>
        /// <param name="handler"></param>
        /// <returns><see cref="NetMQMessageContext{TSocket}"/>NetMqContext</returns>
        internal static ContextType GetContextType(MethodInfo handler)
        {
            var member = handler.GetParameters().First();
            var t = member.ParameterType;
            return member switch
            {
                var type when member.ParameterType.GetGenericTypeDefinition() == typeof(MessageContext<,>) => ContextType.TypedContext,
                var type when member.ParameterType.GetGenericTypeDefinition() == typeof(NetMQMessageContext<>) => ContextType.NetMqContext,
                _ => throw new ArgumentException($"Invalid Context type")
            };
        }
        
        /// <summary>
        /// Map object to <see cref="MessageContext{TSocket,TMessage}"/>
        /// </summary>
        /// <returns><see cref="MessageContext{TSocket,TMessage}"/></returns>
        internal static object GetTypedContext(MethodInfo handler, NetMQMessage message, NetMQSocket socket)
        {
            var contexttype =  handler.GetParameters().First().ParameterType;
            var messagetype = contexttype.GetGenericArguments().Last();
            var mesg = message.ReadAllFramesAsString();
            object result = null;
            if (!messagetype.IsPrimitive && messagetype != typeof(string))
            {
                result = JsonConvert.DeserializeObject(mesg);
            }
            else
            {
                result = MapPrimitive(mesg);
            }
            if (result.GetType() == messagetype)
            {
                var context = Activator.CreateInstance(contexttype);
                context.GetType().GetProperty("Message").SetValue(context, result);
                context.GetType().GetProperty("Socket").SetValue(context, socket);
                context.GetType().GetProperty("RoutingKey").SetValue(context, message.First.ConvertToString());
                return context;
            }

            return null;
        }

        private static object MapPrimitive(string mesg)
        {
            object result = null;
            if (int.TryParse(mesg, out var i))
                result = i;
            if (double.TryParse(mesg, out var d))
                result = d;
            if (float.TryParse(mesg, out var f))
                result = f;
            if (long.TryParse(mesg, out var l))
                result = l;
            return result ?? mesg;
        }
    }
   

    internal enum ContextType
    {
        NetMqContext,
        TypedContext,
    }
    
}
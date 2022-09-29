using System;
using System.Linq;
using System.Reflection;
using NetMQ.Controllers.Core;

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
    }

    internal enum ContextType
    {
        NetMqContext,
        TypedContext,
    }
    
}
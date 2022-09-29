using System.Collections.Generic;
using System.Reflection;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Attributes.Filtering;

namespace NetMQ.Controllers.Core.SocketFactories
{
    public interface ISocketFactory<TAttribute> where TAttribute: BaseSocketAttribute
    {
        NetMQSocket BuildSocket(object controllerInstance, MethodInfo handler, IEnumerable<IFilter> filters,
            TAttribute socketAttribute);
    }
}
using System;

namespace NetMQ.Controllers.Attributes.Filtering
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class FilterAttribute : Attribute, IFilter
    {

        public abstract bool IsMatch(NetMQMessage message);
    }
}
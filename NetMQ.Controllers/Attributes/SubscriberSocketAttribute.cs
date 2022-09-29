using System;
using System.Runtime.CompilerServices;

namespace NetMQ.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscriberSocketAttribute : BaseSocketAttribute
    {
        public string Topic { get; set; }
    }
}
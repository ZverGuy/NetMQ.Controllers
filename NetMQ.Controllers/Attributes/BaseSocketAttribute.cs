using System;

namespace NetMQ.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BaseSocketAttribute : Attribute
    {
        public string ConnectionString { get; set; }
    }
}
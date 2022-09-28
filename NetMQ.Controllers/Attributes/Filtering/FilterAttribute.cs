using System;

namespace NetMQ.Controllers.Attributes.Filtering
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilterAttribute : Attribute
    {
        public IFilter Filter { get; set; }
    }
}
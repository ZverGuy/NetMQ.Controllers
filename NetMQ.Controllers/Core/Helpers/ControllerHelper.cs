using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using NetMQ.Controllers.Attributes;
using NetMQ.Controllers.Attributes.Filtering;
using NetMQ.Controllers.Core.SocketFactories;

namespace NetMQ.Controllers
{
    internal static class ControllerHelper
    {
        internal static List<Type> _controllerCache { get; } = new List<Type>();
        internal static List<Type> _factoryCache = new List<Type>();

        /// <summary>
        /// Get All Classes that use derived class from <see cref="BaseSocketAttribute"/>
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Type> GetControllers()
        {
           

            if (!_controllerCache.Any())
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var types = assemblies.SelectMany(x => x.GetTypes());
                _controllerCache.AddRange(types.Where(x =>
                {
                    return x
                        .GetMethods()
                        .Any(z => z.GetCustomAttributes(typeof(BaseSocketAttribute), true).Length > 0);
                }));
            }

            return _controllerCache;
        }
        internal static IEnumerable<Type> GetSocketFactories()
        {
            
            if (!_factoryCache.Any())
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var types = assemblies.SelectMany(x => x.GetTypes());
                _factoryCache.AddRange(types.Where(x =>
                {
                    return x.GetInterfaces()
                        .Any(l => l.IsGenericType && l.GetGenericTypeDefinition() == typeof(ISocketFactory<>));
                }));
            }
            
            return _factoryCache;
        }

        internal static IEnumerable<MethodInfo> GetMethodsThatHaveSocketAttributes<TSocketType>()
            where TSocketType : BaseSocketAttribute
        {
            return _controllerCache.SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(TSocketType), true).Length > 0);
        }

        internal static IEnumerable<BaseSocketAttribute> GetSocketType(MethodInfo info)
        {
            return info.GetCustomAttributes(typeof(BaseSocketAttribute), inherit: true).Cast<BaseSocketAttribute>();
        }

        internal static IEnumerable<MethodInfo> GetMethodsThatHaveSocketAttributes(object controller)
        {
            var type = controller.GetType();
            return type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(x => 
                    x.GetCustomAttributes()
                        .Any(l => l is BaseSocketAttribute))
                    ;
        }

        internal static IEnumerable<IFilter> GetFilters(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(FilterAttribute), inherit: true)
                .Cast<FilterAttribute>();
        }
    }
}
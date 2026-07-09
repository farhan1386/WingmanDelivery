using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace WingmanDelivery.Server.Extensions
{
    public static class EndpointRegistrationExtensions
    {
        public static IEndpointRouteBuilder MapAllEndpointModules(this IEndpointRouteBuilder app)
        {
            var modules = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IEndpointModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IEndpointModule>();

            foreach (var module in modules)
            {
                module.MapEndpoints(app);
            }

            return app;
        }
    }
}

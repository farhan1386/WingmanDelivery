using Microsoft.AspNetCore.Routing;

namespace WingmanDelivery.Server.Extensions
{
    public interface IEndpointModule
    {
        void MapEndpoints(IEndpointRouteBuilder app);
    }
}
using API.Endpoints;

namespace API.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapProductEndpoints();
        return endpoints;
    }
}

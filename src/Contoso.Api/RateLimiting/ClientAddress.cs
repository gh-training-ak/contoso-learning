using Microsoft.AspNetCore.Http;

namespace Contoso.Api.RateLimiting;

public static class ClientAddress
{
    private const string ForwardedFor = "X-Forwarded-For";

    public static string Resolve(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(ForwardedFor, out var header))
        {
            var first = header.ToString().Split(',', StringSplitOptions.TrimEntries).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(first))
            {
                return first;
            }
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

using System.Net;
using System.Text.Json;

namespace DBro.Web.Handlers;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                context.Response.Redirect("/not-found");
                return;
            }
            else if (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                context.Response.Redirect("/bad-request");
                return;
            }
            else if (ex.StatusCode == HttpStatusCode.InternalServerError)
            {
                context.Response.Redirect("/internal-server-error");
                return;
            }
        }
        catch (JsonException)
        {
            var cookies = context.Request.Cookies;
            foreach (var cookie in cookies.Keys) context.Response.Cookies.Delete(cookie);
            context.Response.Redirect("/login");
            return;
        }
    }
}

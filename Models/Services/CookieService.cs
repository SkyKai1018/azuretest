using azuretest.Models;
using System.Text.Json;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<Filter> GetFiltersFromCookie()
    {
        var filtersCookie = _httpContextAccessor.HttpContext.Request.Cookies["Filters"];
        if (filtersCookie != null)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new FilterConverter() }
            };
            return JsonSerializer.Deserialize<List<Filter>>(filtersCookie, options);
        }
        return new List<Filter>();
    }

    public void SaveFiltersToCookie(List<Filter> filters)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new FilterConverter() }
        };

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(30)
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append("Filters", JsonSerializer.Serialize(filters, options), cookieOptions);
    }
}

using azuretest.Models;

public interface ICookieService
{
    List<Filter> GetFiltersFromCookie();
    void SaveFiltersToCookie(List<Filter> filters);
}

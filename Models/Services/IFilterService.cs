using azuretest.Models;

public interface IFilterService
{
    List<Stock> StartFilter(List<Filter> filters);
}
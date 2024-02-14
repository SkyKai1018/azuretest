using azuretest.Models;

public interface IFilterService
{
    void AddFilter(Filter filter);
    List<Stock> StartFilter();
    void DeleteFilterStrategy(int id);
}
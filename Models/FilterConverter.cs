using azuretest.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FilterConverter : JsonConverter<Filter>
{
    public override Filter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(ref reader);
        if (jsonElement.TryGetProperty("FilterType", out var typeProperty))
        {
            var filterType = typeProperty.GetString();
            switch (filterType)
            {
                case "StockPriceFilter":
                    return JsonSerializer.Deserialize<StockPriceFilter>(jsonElement.GetRawText(), options);
                case "RiseFilter":
                    return JsonSerializer.Deserialize<RiseFilter>(jsonElement.GetRawText(), options);
                case "FallFilter":
                    return JsonSerializer.Deserialize<FallFilter>(jsonElement.GetRawText(), options);
                case "DaysChangeFilter":
                    return JsonSerializer.Deserialize<DaysChangeFilter>(jsonElement.GetRawText(), options);
            }
        }
        throw new JsonException("Unknown filter type.");
    }

    public override void Write(Utf8JsonWriter writer, Filter value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}
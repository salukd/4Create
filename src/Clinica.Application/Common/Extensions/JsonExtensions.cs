using Newtonsoft.Json;

namespace Clinica.Application.Common.Extensions;

public static class JsonExtensions
{
    public static T? Deserialize<T>(this string json) where T : class
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return null;
        }
    }
}
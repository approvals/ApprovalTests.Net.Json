using System.IO;
using System.Text;
using Newtonsoft.Json;
using ObjectApproval;

public static partial class ObjectApprover
{
    public static string AsFormattedJson(object target, JsonSerializerSettings jsonSerializerSettings = null)
    {
        var serializer = GetJsonSerializer(jsonSerializerSettings);
        var builder = new StringBuilder();
        using (var stringWriter = new StringWriter(builder))
        using (var writer = new JsonTextWriter(stringWriter))
        {
            writer.QuoteChar = '\'';
            writer.QuoteName = false;
            serializer.Serialize(writer, target);
        }

        builder.Replace(@"\\", @"\");
        return builder.ToString();
    }

    static JsonSerializer GetJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
    {
        if (jsonSerializerSettings == null)
        {
            return JsonSerializer.Create(SerializerBuilder.BuildSettings());
        }

        return JsonSerializer.Create(jsonSerializerSettings);
    }
}
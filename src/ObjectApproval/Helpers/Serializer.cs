using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public static class Serializer
    {
        public static string Serialize(this JsonSerializer jsonSerializer, object target)
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = jsonSerializer.Formatting;
                    jsonSerializer.Serialize(jsonWriter, target);
                }

                return stringWriter.ToString();
            }
        }
    }
}
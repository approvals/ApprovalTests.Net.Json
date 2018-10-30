using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public class StringScrubber : JsonConverter
    {
        static List<string> datetimeFormats = new List<string>();
        static List<string> datetimeOffsetFormats = new List<string>();

        public static void AddExtraDatetimeFormat(string format)
        {
            datetimeFormats.Add(format);
        }

        public static void AddExtraDatetimeOffsetFormat(string format)
        {
            datetimeOffsetFormats.Add(format);
        }

        Scrubber<Guid> guidScrubber;
        Scrubber<DateTime> dateTimeScrubber;
        Scrubber<DateTimeOffset> dateTimeOffsetScrubber;

        public StringScrubber(Scrubber<Guid> guidScrubber, Scrubber<DateTime> dateTimeScrubber, Scrubber<DateTimeOffset> dateTimeOffsetScrubber)
        {
            this.guidScrubber = guidScrubber;
            this.dateTimeScrubber = dateTimeScrubber;
            this.dateTimeOffsetScrubber = dateTimeOffsetScrubber;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueAsString = (string) value;
            if (!string.IsNullOrWhiteSpace(valueAsString))
            {
                if (guidScrubber != null)
                {
                    if (Guid.TryParse(valueAsString, out var guid))
                    {
                        guidScrubber.WriteValue(writer, guid);
                        return;
                    }
                }

                if (dateTimeScrubber != null)
                {
                    if (DateTimeOffset.TryParse(valueAsString, out var dateTimeOffset))
                    {
                        dateTimeOffsetScrubber.WriteValue(writer, dateTimeOffset);
                        return;
                    }

                    foreach (var format in datetimeOffsetFormats)
                    {
                        if (DateTimeOffset.TryParseExact(valueAsString, format, null, DateTimeStyles.None, out dateTimeOffset))
                        {
                            dateTimeOffsetScrubber.WriteValue(writer, dateTimeOffset);
                            return;
                        }
                    }

                    if (DateTime.TryParse(valueAsString, out var dateTime))
                    {
                        dateTimeScrubber.WriteValue(writer, dateTime);
                        return;
                    }

                    foreach (var format in datetimeFormats)
                    {
                        if (DateTime.TryParseExact(valueAsString, format, null, DateTimeStyles.None, out dateTime))
                        {
                            dateTimeScrubber.WriteValue(writer, dateTime);
                            return;
                        }
                    }
                }
            }

            writer.WriteValue(valueAsString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new Exception();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type type)
        {
            return type == typeof(string);
        }
    }
}
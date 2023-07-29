using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MaxxWebApplication.Health;

public static class HealthResponseFormatter
{
    public static Task WriteResponse(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var options = new JsonWriterOptions { Indented = true };
        using var utf8Json = new MemoryStream();
        using (var utf8JsonWriter1 = new Utf8JsonWriter(utf8Json, options))
        {
            utf8JsonWriter1.WriteStartObject();
            utf8JsonWriter1.WriteString("status", result.Status.ToString());
            utf8JsonWriter1.WriteStartObject("results");
            foreach (var entry in result.Entries)
            {
                utf8JsonWriter1.WriteStartObject(entry.Key);
                var utf8JsonWriter2 = utf8JsonWriter1;
                var healthReportEntry = entry.Value;
                var str1 = healthReportEntry.Status.ToString();
                utf8JsonWriter2.WriteString("status", str1);
                var utf8JsonWriter3 = utf8JsonWriter1;
                healthReportEntry = entry.Value;
                var description = healthReportEntry.Description;
                utf8JsonWriter3.WriteString("description", description);
                healthReportEntry = entry.Value;
                if (healthReportEntry.Exception != null)
                {
                    var utf8JsonWriter4 = utf8JsonWriter1;
                    healthReportEntry = entry.Value;
                    var str2 = healthReportEntry.Exception.ToString();
                    utf8JsonWriter4.WriteString("exception", str2);
                }

                utf8JsonWriter1.WriteStartObject("data");
                healthReportEntry = entry.Value;
                foreach (var keyValuePair in healthReportEntry.Data)
                {
                    utf8JsonWriter1.WritePropertyName(keyValuePair.Key);
                    var writer = utf8JsonWriter1;
                    var obj = keyValuePair.Value;
                    var inputType = keyValuePair.Value?.GetType();
                    if ((object)inputType == null)
                    {
                        inputType = typeof(object);
                    }

                    JsonSerializer.Serialize(writer, obj, inputType);
                }

                utf8JsonWriter1.WriteEndObject();
                utf8JsonWriter1.WriteEndObject();
            }

            utf8JsonWriter1.WriteEndObject();
            utf8JsonWriter1.WriteEndObject();
        }

        var text = Encoding.UTF8.GetString(utf8Json.ToArray());

        return context.Response.WriteAsync(text);
    }
}

using System.Text;

using Confluent.Kafka;

namespace Kindred.Rewards.Core.Infrastructure.Queue.Extensions;

public static class ByteExtensions
{
    public static string AsString(this byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] AsBytes(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }
}
public static class KafkaExtensions
{
    public static string GetCorrelationId(this Headers kafkaHeaders)
    {
        return kafkaHeaders.Get("correlationId");
    }

    public static string Get(this Headers kafkaHeaders, string key)
    {
        var header = (Header)kafkaHeaders.FirstOrDefault(x => x.Key == key);
        return header?.GetValueBytes().AsString();
    }
}

using Newtonsoft.Json;
using System;

namespace BankId.Merchant.Library.SampleWebsite
{
    public interface IDecimalModelBinder
    {
        bool CanRead { get; }
        bool CanWrite { get; }

        bool CanConvert(Type objectType);
        object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);
        void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
    }
}
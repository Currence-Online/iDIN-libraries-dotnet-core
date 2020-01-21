using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankId.Merchant.Library.SampleWebsite
{
    public class DecimalModelBinder : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => objectType == typeof(decimal);

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return token.ToObject<decimal>();
            }

            if (token.Type == JTokenType.String)
            {
                var val = token.ToString();
                val = val.Replace(",", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                val = val.Replace(".", CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);

                return Decimal.Parse(val);
            }

            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }

            throw new JsonSerializationException("Unexpected token type: " +
                                                 token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
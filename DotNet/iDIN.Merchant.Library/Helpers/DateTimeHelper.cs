using System;
using System.Globalization;
using System.Xml;

namespace BankId.Merchant.Library.Helpers
{
    internal static class DateTimeHelper
    {
        internal static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        internal static string ProcessDateTimes(string input, string[] dateTimeElementNames)
        {
            var doc = new XmlDocument {PreserveWhitespace = true};
            doc.LoadXml(input);

            foreach (var elementName in dateTimeElementNames)
            {
                var tagName = elementName;
                string attributeName = null;

                var attributeIndex = elementName.IndexOf("@", StringComparison.Ordinal);
                if (attributeIndex > 0)
                {
                    tagName = elementName.Substring(0, attributeIndex);
                    attributeName = elementName.Substring(attributeIndex + 1, elementName.Length - attributeIndex - 1);
                }

                foreach (XmlElement element in doc.GetElementsByTagName(tagName, "*"))
                {
                    if (attributeIndex == -1)
                    {
                        var existing = DateTime.Parse(element.InnerText, CultureInfo.InvariantCulture).ToUniversalTime();
                        var newval = existing.ToIdxFormat();
                        element.InnerText = newval;
                    }
                    else
                    {
                        var existing = DateTime.Parse(element.Attributes[attributeName].Value, CultureInfo.InvariantCulture).ToUniversalTime();
                        var newval = existing.ToIdxFormat();
                        element.Attributes[attributeName].Value = newval;
                    }
                }
            }

            return doc.OuterXml;
        }
    }
}

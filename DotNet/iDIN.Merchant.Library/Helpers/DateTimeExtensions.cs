using System;

namespace BankId.Merchant.Library.Helpers
{
    internal static class DateTimeExtensions
    {
        internal static string ToIdxFormat(this DateTime value)
        {
            return value.ToString(Constants.DateTimeIDxFormat);
        }
    }
}

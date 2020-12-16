using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;

namespace IEvangelist.Blazing.WarFleet.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static bool TryGetQueryString<T>(
            this NavigationManager navManager, string key, out T value)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
            {
                value = Parse<T>(valueFromQueryString.ToString());
                return true;

                static TValue Parse<TValue>(string val) => typeof(TValue).FullName switch
                {
                    "System.Enum" => (TValue)(Enum.TryParse(typeof(TValue), val, out var result) ? result : default),
                    "System.Int32" => (TValue)(object)val.ToInt32(),
                    "System.Boolean" => (TValue)(object)val.ToBool(),
                    "System.Decimal" => (TValue)(object)val.ToDecimal(),
                    "System.String" => (TValue)(object)val,

                    _ => throw new Exception("Nope...")
                };
            }

            value = default!;
            return false;
        }

        delegate bool TryParseDelegate<T>(string s, out T result);

        static T To<T>(string value, TryParseDelegate<T> parse) where T : struct
            => parse(value, out var result) ? result : default;

        static int ToInt32(this string value) => To<int>(value, int.TryParse);
        static bool ToBool(this string value) => To<bool>(value, bool.TryParse);
        static decimal ToDecimal(this string value) => To<decimal>(value, decimal.TryParse);
    }
}

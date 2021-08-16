using System;
using Newtonsoft.Json;

namespace Easify.Exports.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T t) where T: class
        {
            if (t == null)
                return string.Empty;

            try
            {
                return JsonConvert.SerializeObject(t);
            }
            catch (Exception e)
            {
                return t.ToString();
            }
        }
    }
}
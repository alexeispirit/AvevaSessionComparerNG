using System;
using System.Linq;
using Aveva.Core.Database;

namespace SessionCompareNG
{
    public static class ObjectAsString
    {
        public static string ToString(object obj)
        {
            string value = string.Empty;

            switch (obj)
            {
                case string[] stringArray:
                    value = String.Join(";", stringArray);
                    break;
                case int[] intArray:
                    value = String.Join(";", intArray.Select(i => i.ToString()).ToArray());
                    break;
                case double[] doubleArray:
                    value = String.Join(";", doubleArray.Select(i => i.ToString()).ToArray());
                    break;
                case bool[] boolArray:
                    value = String.Join(";", boolArray.Select(i => i.ToString()).ToArray());
                    break;
                case DbElement[] dbElementArray:
                    value = String.Join(";", dbElementArray.Select(i => i.Name()).ToArray());
                    break;
                default:
                    value = obj != null ? obj.ToString() : string.Empty;
                    break;
            }

            return value.Length > 1000 ? value.Substring(0, 1000) : value;
        }
    }
}

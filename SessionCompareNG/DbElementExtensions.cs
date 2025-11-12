using System;
using System.Linq;
using Aveva.Core.Database;

namespace SessionCompareNG
{
    public static class DbElementExtensions
    {
        public static string GetRef(this DbElement dbElement)
        {
            if (!dbElement.IsNull)
            {
                return $"={String.Join("/", dbElement.Ref)}";
            }
            else
            {
                return "";
            }
        }

        public static string GetAttributeValueAsString(this DbElement dbElement, DbAttribute attribute)
        {
            string value = string.Empty;

            if (attribute.IsArray)
            {
                switch (attribute.Type)
                {
                    case DbAttributeType.INTEGER:
                        int[] valuesInt = dbElement.GetIntegerArray(attribute);
                        value = String.Join(";", valuesInt.Select(i => i.ToString()).ToArray());
                        break;
                    case DbAttributeType.DOUBLE:
                        double[] valuesDbl = dbElement.GetDoubleArray(attribute);
                        value = String.Join(";", valuesDbl.Select(i => i.ToString()).ToArray());
                        break;
                    case DbAttributeType.BOOL:
                        bool[] valuesBool = dbElement.GetBoolArray(attribute);
                        value = String.Join(";", valuesBool.Select(i => i.ToString()).ToArray());
                        break;
                    case DbAttributeType.WORD:
                    case DbAttributeType.STRING:
                    case DbAttributeType.STRINGARRAY:
                        string[] valuesStr = dbElement.GetStringArray(attribute);
                        value = String.Join(";", valuesStr);
                        break;
                    case DbAttributeType.ELEMENT:
                        DbElement[] valuesEl = dbElement.GetElementArray(attribute);
                        value = String.Join(";", valuesEl.Select(i => i.Name()).ToArray());
                        break;
                    default:
                        try
                        {
                            string[] anyStr = dbElement.GetAsStringArray(attribute);
                            value = String.Join(";", anyStr);
                        }
                        catch 
                        {
                            Console.WriteLine($"Error processing attribute {attribute.Name} of type {attribute.Type.ToString()}");
                        }
                        break;
                }
            }
            else
            {
                value = dbElement.GetAsString(attribute);
            }

            return value.Length > 1000 ? value.Substring(0, 999) : value;
        }
    }
}

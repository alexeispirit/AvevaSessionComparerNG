using System.Collections;

namespace SessionCompareNG
{
    internal class Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override string ToString()
        {
            return $"{Name}: {OldValue} -> {NewValue}";
        }

        public Hashtable ToHashtable()
        {
            Hashtable result = new Hashtable();
            result[1] = Name;
            result[2] = Description;
            result[3] = OldValue;
            result[4] = NewValue;

            return result;
        }
    }
}

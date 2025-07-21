using System.Text.RegularExpressions;

namespace SessionCompareNG
{
    public class Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Attribute(string name, string description, string value)
        {
            Name = name;
            Description = description;
            Value = Regex.Replace(value, @"\p{C}+", string.Empty);
        }
    }
}

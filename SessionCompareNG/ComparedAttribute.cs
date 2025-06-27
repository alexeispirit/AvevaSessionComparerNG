using System.Collections;
using Aveva.Core.Utilities.CommandLine;

namespace SessionCompareNG
{
    public class ComparedAttribute
    {
        public string Name { get; set; }
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
            result[2] = OldValue;
            result[3] = NewValue;

            return result;
        }

        public Command ToAvevaCommand()
        {
            Command command = Command.CreateCommand($"$P {Name}: {OldValue} -> {NewValue}");
            return command;
        }
    }
}

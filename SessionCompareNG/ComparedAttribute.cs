using System;
using System.Xml;
using System.Collections;
using Aveva.Core.Utilities.CommandLine;

namespace SessionCompareNG
{
    public class ComparedAttribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AttributeState State { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public ComparedAttribute(string name, string description, string oldValue, string newValue)
        {
            Name = name;
            Description = description;
            OldValue = oldValue;
            NewValue = newValue;
            State = OldValue == NewValue ? AttributeState.Idle : AttributeState.Modified;
        }

        public override string ToString()
        {
            return $"{Name} ({Description}): {OldValue} -> {NewValue}";
        }

        public void DebugPrint()
        {
            Console.WriteLine(this.ToString());
        }

        public Hashtable ToHashtable()
        {
            Hashtable result = new Hashtable();
            result[1] = Name;
            result[2] = Description;
            result[3] = OldValue;
            result[4] = NewValue;
            result[5] = State.ToString();

            return result;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Attribute");
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Description", Description);
            writer.WriteAttributeString("State", State.ToString());
            writer.WriteAttributeString("OldValue", OldValue);
            writer.WriteAttributeString("NewValue", NewValue);
            writer.WriteEndElement();
        }

        public Command ToAvevaCommand()
        {
            Command command = Command.CreateCommand($"$P {Name}: {OldValue} -> {NewValue}");
            return command;
        }
    }
}

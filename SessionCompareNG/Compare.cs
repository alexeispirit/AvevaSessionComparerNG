using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aveva.Core.PMLNet;
using Aveva.Core.Utilities.CommandLine;

namespace SessionCompareNG
{
    [PMLNetCallable]
    public class Compare
    {
        public TagInfo Tag1;
        public TagInfo Tag2;
        public List<ComparedAttribute> ComparedAttributes = new List<ComparedAttribute>();

        [PMLNetCallable]
        public Compare() { }

        [PMLNetCallable]
        public Compare(TagInfo tag1, TagInfo tag2)
        {
            Tag1 = tag1;
            Tag2 = tag2;
            RunCompare();
        }

        [PMLNetCallable]
        public void Assign(Compare other) 
        {
            Tag1 = other.Tag1;
            Tag2 = other.Tag2;
            ComparedAttributes = other.ComparedAttributes;
        }

        [PMLNetCallable]
        public Hashtable Attributes()
        {
            Hashtable result = new Hashtable();
            int i = 0;
            foreach (ComparedAttribute comparedAttribute in ComparedAttributes)
            {
                result[++i] = comparedAttribute.ToHashtable();
            }

            return result;
        }

        [PMLNetCallable]
        public Hashtable Attribute(string name)
        {
            Hashtable result = new Hashtable();
            ComparedAttribute att = ComparedAttributes.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (att != null)
            {
                result[1] = att.Name;
                result[2] = att.OldValue;
                result[3] = att.NewValue;
                return result;
            }
            else
            {
                return result;
            }
        }

        [PMLNetCallable]
        public Hashtable Difference()
        {
            Hashtable result = new Hashtable();
            int i = 0;
            foreach (ComparedAttribute comparedAttribute in ComparedAttributes.Where(x => x.OldValue != x.NewValue))
            {
                result[++i] = comparedAttribute.ToHashtable();
            }

            return result;
        }

        [PMLNetCallable]
        public void Print()
        {
            Command command = Command.CreateCommand($"$P {Tag1.Name}");
            command.RunInPdms();
            foreach (ComparedAttribute attCompared in ComparedAttributes.Where(x => x.OldValue != x.NewValue))
            {
                command = attCompared.ToAvevaCommand();
                command.RunInPdms();
            }
        }

        private void RunCompare()
        {
            if (Tag1.IsNull || Tag2.IsNull)
            {
                throw new Exception("Two TagInfo objects required.");
            }

            foreach (string key in Tag1.AttributesDict.Keys)
            {
                ComparedAttribute comparedAttribute = new ComparedAttribute
                {
                    Name = key,
                    OldValue = Tag1.AttributesDict[key],
                    NewValue = Tag2.AttributesDict[key]
                };
                ComparedAttributes.Add(comparedAttribute);
            }
        }


    }
}

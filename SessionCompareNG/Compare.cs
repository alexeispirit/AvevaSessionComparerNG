using System.Collections;
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
        public ComparedTag CompareResult;

        [PMLNetCallable]
        public Compare() { }

        [PMLNetCallable]
        public Compare(TagInfo tag1, TagInfo tag2)
        {
            Tag1 = tag1;
            Tag2 = tag2;
            CompareResult = new ComparedTag(tag1, tag2);
        }

        [PMLNetCallable]
        public void Assign(Compare other) 
        {
            Tag1 = other.Tag1;
            Tag2 = other.Tag2;
            CompareResult = other.CompareResult;
        }

        [PMLNetCallable]
        public Hashtable Attributes()
        {
            Hashtable result = new Hashtable();
            int i = 0;
            foreach (ComparedAttribute comparedAttribute in CompareResult.Attributes)
            {
                result[++i] = comparedAttribute.ToHashtable();
            }

            return result;
        }

        [PMLNetCallable]
        public Hashtable Attribute(string name)
        {
            Hashtable result = new Hashtable();
            ComparedAttribute att = CompareResult.Attributes.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
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
            foreach (ComparedAttribute comparedAttribute in CompareResult.Attributes.Where(x => x.State == AttributeState.Modified))
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
            foreach (ComparedAttribute attCompared in CompareResult.Attributes.Where(x => x.State == AttributeState.Modified))
            {
                command = attCompared.ToAvevaCommand();
                command.RunInPdms();
            }
        }
    }
}

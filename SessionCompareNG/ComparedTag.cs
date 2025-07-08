using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Aveva.Core.Database;

namespace SessionCompareNG
{
    public class ComparedTag
    {
        public string Name { get; set; }
        public TagState State {  get; set; }
        public TagInfo PreviousSessionTag { get; set; }
        public TagInfo CurrentSessionTag { get; set; }
        public List<ComparedAttribute> Attributes { get; set; }
        public Session PreviousSession { get; set; }
        public Session CurrentSession { get; set; }

        public ComparedTag(TagInfo prevSessionTag, TagInfo currSessionTag, TagState state = TagState.Uncompared) 
        {
            if (prevSessionTag.IsNull || currSessionTag.IsNull) 
            {
                throw new Exception("Two TagInfo objects required.");
            }

            if (prevSessionTag.Name != currSessionTag.Name)
            {
                throw new Exception("Tags have different names.");
            }

            if (prevSessionTag.UDET.Name != currSessionTag.UDET.Name)
            {
                throw new Exception("Tags have different UDET.");
            }

            Init(prevSessionTag, currSessionTag, state);
        }

        public ComparedTag(TagInfo someSessionTag, TagState state = TagState.Uncompared)
        {
            if (someSessionTag.IsNull)
            {
                throw new Exception("Tag element required.");
            }

            Init(someSessionTag, someSessionTag, state);
        }

        public void Init(TagInfo prevSessionTag, TagInfo currSessionTag, TagState state)
        {
            PreviousSessionTag = prevSessionTag;
            CurrentSessionTag = currSessionTag;
            Name = CurrentSessionTag.Name;
            State = state;

            Attributes = new List<ComparedAttribute>();

            foreach (DbAttribute dbAttrKey in CurrentSessionTag.PossibleAttributes)
            {
                string attrName = dbAttrKey.Name;
                string attrDesc = dbAttrKey.Description;
                ComparedAttribute comparedAttribute = new ComparedAttribute(
                    attrName,
                    attrDesc,
                    PreviousSessionTag.AttributesDict[attrName.ToLower()].Value,
                    CurrentSessionTag.AttributesDict[attrName.ToLower()].Value);
                Attributes.Add(comparedAttribute);
            }

            DbSession prevSession = PreviousSessionTag.Session;
            PreviousSession = new Session(prevSession.User, prevSession.SessionNumber, prevSession.Date);

            DbSession currSession = CurrentSessionTag.Session;
            CurrentSession = new Session(currSession.User, currSession.SessionNumber, currSession.Date);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Tag");
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("State", State.ToString());
            PreviousSession.WriteXml(writer, SessionType.Previous);
            CurrentSession.WriteXml(writer, SessionType.Current);
            writer.WriteStartElement("Attributes");
            Attributes.ForEach(attr => attr.WriteXml(writer));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public void DebugPrint()
        {
            Console.WriteLine($"{Name} [{State.ToString()}]");
            switch(State)
            {
                case TagState.New:
                case TagState.Deleted:
                case TagState.Idle:
                    CurrentSession.DebugPrint();
                    break;
                case TagState.Modified:
                case TagState.Uncompared:
                default:
                    PreviousSession.DebugPrint();
                    CurrentSession.DebugPrint();
                    break;
            }
            foreach (ComparedAttribute att in Attributes.Where(x => x.State == AttributeState.Modified))
            {
                att.DebugPrint();
            }
        }
    }
}

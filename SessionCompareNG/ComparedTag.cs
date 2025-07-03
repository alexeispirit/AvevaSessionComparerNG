using System;
using System.Collections.Generic;
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
                    PreviousSessionTag.AttributesDict[attrName.ToLower()],
                    CurrentSessionTag.AttributesDict[attrName.ToLower()]);
                Attributes.Add(comparedAttribute);
            }

            DbSession prevSession = PreviousSessionTag.Session;
            PreviousSession = new Session(prevSession.User, prevSession.SessionNumber, prevSession.Date);
            
            DbSession currSession = CurrentSessionTag.Session;
            CurrentSession = new Session(currSession.User, currSession.SessionNumber, currSession.Date);
        }
    }
}

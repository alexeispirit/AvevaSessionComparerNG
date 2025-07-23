using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using Aveva.Core.Database;
using Aveva.Engineering.Grids.Implementation;

namespace SessionCompareNG
{
    public class ComparedTagsList
    {
        public string Db { get; set; }
        public string UDET { get; set; }
        public ListDefinition ListDefinition { get; set; }
        public List<TagSession> PreviousSessionList { get; set; }
        public List<TagSession> CurrentSessionList { get; set; }
        public List<ComparedTag> Idle { get; set; }
        public List<ComparedTag> Modified { get; set; }
        public List<ComparedTag> New { get; set; }
        public List<ComparedTag> Deleted { get; set; }

        public ComparedTagsList(string[] previous, string[] current, ListDefinition lstref)
        {
            ListDefinition = lstref;
            Init(previous, current);
        }

        public ComparedTagsList(string[] previous, string[] current, string lstref)
        {
            DefineListDef(lstref);
            Init(previous, current);
        }

        public void Init(string[] previous, string[] current)
        {
            if (current.Length < 1)
            {
                throw new Exception("Document in current session has no content");
            }

            PreviousSessionList = previous.Select(x => new TagSession(x)).ToList();
            CurrentSessionList = current.Select(x => new TagSession(x)).ToList();

            DefineProperties(CurrentSessionList.First().Tag);

            List<string> prevTags = PreviousSessionList.Select(x => x.Tag).Distinct().ToList();
            List<string> currTags = CurrentSessionList.Select(x => x.Tag).Distinct().ToList();

            List<TagSession> newTags = CurrentSessionList.Where(x => !prevTags.Contains(x.Tag)).ToList();
            List<TagSession> deletedTags = PreviousSessionList.Where(x => !currTags.Contains(x.Tag)).ToList();

            List<TagSession> idleTags = CurrentSessionList.Join(
                PreviousSessionList,
                ts1 => new { ts1.Tag, ts1.Session },
                ts2 => new { ts2.Tag, ts2.Session },
                (ts1, ts2) => ts1
                ).ToList();

            List<TagSession> modifiedTags = CurrentSessionList.Join(
                PreviousSessionList,
                ts1 => ts1.Tag,
                ts2 => ts2.Tag,
                (ts1, ts2) => new { TS1 = ts1, TS2 = ts2 }
                ).Where(x => x.TS1.Session != x.TS2.Session)
                .Select(x => new TagSession
                {
                    Tag = x.TS1.Tag,
                    Session = x.TS1.Session,
                    PreviousSession = x.TS2.Session,
                })
                .ToList();

            New = CompareTags(newTags, TagState.New);
            Deleted = CompareTags(deletedTags, TagState.Deleted);
            Idle = CompareTags(idleTags, TagState.Idle);
            Modified = CompareTags(modifiedTags, TagState.Modified);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Tags");
            New.ForEach(tag => tag.WriteXml(writer));
            Modified.ForEach(tag => tag.WriteXml(writer));
            Idle.ForEach(tag => tag.WriteXml(writer));
            Deleted.ForEach(tag => tag.WriteXml(writer));
            writer.WriteEndElement();
        }

        public void DebugPrint()
        {
            Console.WriteLine("===============");
            Console.WriteLine("NEW TAGS:");
            New.ForEach(x => x.DebugPrint());
            Console.WriteLine("===============");
            Console.WriteLine("MODIFIED TAGS:");
            Modified.ForEach(x => x.DebugPrint());
            Console.WriteLine("===============");
            Console.WriteLine("IDLE TAGS:");
            Idle.ForEach(x => x.DebugPrint());
            Console.WriteLine("===============");
            Console.WriteLine("DELETED TAGS:");
            Deleted.ForEach(x => x.DebugPrint());
            Console.WriteLine("===============");
        }

        private List<ComparedTag> CompareTags(List<TagSession> tags, TagState state)
        {
            List<ComparedTag> comparedTags = new List<ComparedTag>();

            if (tags == null)
            {
                return comparedTags;
            }

            foreach (TagSession tagSession in tags)
            {
                ComparedTag comparedTag;
                switch (state)
                {
                    case TagState.New:
                    case TagState.Idle:
                    case TagState.Deleted:
                        TagInfo tagInfo;
                        if (ListDefinition != null)
                        {
                            tagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.Session, ListDefinition);
                        }
                        else
                        {
                            tagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.Session);
                        }
                        comparedTag = new ComparedTag(tagInfo, state);
                        break;
                    case TagState.Modified:
                    default:
                        TagInfo prevTagInfo;
                        TagInfo currTagInfo;
                        if (ListDefinition != null)
                        {
                            prevTagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.PreviousSession, ListDefinition);
                            currTagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.Session, ListDefinition);
                        }
                        else
                        {
                            prevTagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.PreviousSession);
                            currTagInfo = new TagInfo(Db, UDET, tagSession.Tag, tagSession.Session);
                        }
                        comparedTag = new ComparedTag(prevTagInfo, currTagInfo, state);
                        break;
                }
                comparedTags.Add(comparedTag);
            }

            return comparedTags;
        }

        private void DefineProperties(string tagName)
        {
            DbElement dbElement = DbElement.GetElement(tagName);
            if (dbElement.IsNull)
            {
                throw new Exception("Cannot define element database");
            }

            Db = dbElement.Db.Name;
            UDET = dbElement.GetActualType().Name;
        }

        private void DefineListDef(string lstRef)
        {
            DbElement dbElement = DbElement.GetElement(lstRef);
            if (!dbElement.IsNull && dbElement.ElementType == DbElementTypeInstance.LSTDEF)
            {
                ListDefinition = new ListDefinition(dbElement);
            }
        }
    }
}

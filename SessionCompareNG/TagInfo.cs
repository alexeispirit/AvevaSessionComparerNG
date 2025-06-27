using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aveva.Core.Database;
using Aveva.Core.PMLNet;
using Aveva.Core.Utilities.CommandLine;

namespace SessionCompareNG
{
    [PMLNetCallable]
    public class TagInfo
    {
        public Db Db;
        public DbElementType UDET;
        public DbElement DbElement;
        public string RefNo;
        public string Name;
        public int SessionNumber;
        public bool IsNull;
        public List<DbAttribute> PossibleAttributes = new List<DbAttribute>();
        public Dictionary<string, string> AttributesDict = new Dictionary<string, string>();

        [PMLNetCallable]
        public TagInfo() { }

        [PMLNetCallable]
        public TagInfo(string dbName, string udetName, string tagName, double sessionNo) 
        {
            Init(dbName, udetName, tagName, sessionNo);
        }

        [PMLNetCallable]
        public void Assign(TagInfo that)
        {
            Db = that.Db;
            UDET = that.UDET;
            DbElement = that.DbElement;
            RefNo = that.RefNo;
            Name = that.Name;
            SessionNumber = that.SessionNumber;
            IsNull = that.IsNull;
            PossibleAttributes = that.PossibleAttributes;
            AttributesDict = that.AttributesDict;
        }

        [PMLNetCallable]
        public Hashtable Attributes()
        {
            Hashtable result = new Hashtable();
            int i = 0;
            foreach (string key in AttributesDict.Keys)
            {
                Hashtable attHashtable = new Hashtable();
                attHashtable[1] = key;
                attHashtable[2] = AttributesDict[key];
                result[++i] = attHashtable;
            }
            return result;
        }

        [PMLNetCallable]
        public string Attribute(string name)
        {
            string attValue = "";
            AttributesDict.TryGetValue(name.ToLower(), out attValue);
            return attValue;
        }

        [PMLNetCallable]
        public void Print()
        {
            foreach (string key in AttributesDict.Keys)
            {
                Command command = Command.CreateCommand($"$P {key}: {AttributesDict[key]}");
                command.RunInPdms();
            }
        }

        private void Init(string dbName, string udetName, string tagName, double sessionNo)
        {
            Name = tagName;
            SessionNumber = (int)sessionNo;
            Db = Database(dbName);
            UDET = Udet(udetName);
            PossibleAttributes = GetPossibleAttributes();
            ProcessElement(Db, Name, SessionNumber);
        }

        private Db Database(string dbName) 
        {
            MDB mdb = MDB.CurrentMDB;
            Db db = mdb.GetDBArray().Where(d => d.Name == dbName).FirstOrDefault();
            if (db == null)
            {
                throw new Exception($"No database found with name {dbName}.");
            }
            return db;
        }

        private DbElementType Udet(string udetName)
        {
            DbElementType[] udets = DbElementType.GetAllUdets();
            DbElementType result = udets.Where(u => u.Name == udetName).FirstOrDefault();
            if (result == null)
            {
                throw new Exception($"No UDET {udetName} found");
            }
            return result;
        }

        private void ProcessElement(Db db, string tagName, int sessionNo)
        {
            DbSession session = db.Session(sessionNo);
            if (!session.IsValid)
            {
                throw new Exception($"Invalid Session in database {db.Name}.");
            }

            db.SwitchToOldSession(session);
            DbElement = DbElement.GetElement(tagName);

            if (!DbElement.IsNull)
            {
                IsNull = false;
                RefNo = GetReference();
                AttributesDict = ProcessAttributes();
            }
            else
            {
                IsNull = true;
            }


            if (db.IsSwitched())
            {
                db.SwitchBackSession(true);
            }
        }

        private List<DbAttribute> GetPossibleAttributes()
        {
            return UDET.DisplayAttributes().ToList();
        }

        private Dictionary<string, string> ProcessAttributes()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("ref", RefNo);

            foreach (DbAttribute dbAttribute in PossibleAttributes)
            {
                string attrName = dbAttribute.Name.ToLower();
                string attrValue = DbElement.GetAsString(dbAttribute);
                attributes.Add(attrName, attrValue);
            }
            
            return attributes;
        }

        private string GetReference()
        {
            if (!IsNull)
            {
                return $"={String.Join("/", DbElement.Ref)}";
            }
            else
            {
                return "";
            }
            
        }
    }
}

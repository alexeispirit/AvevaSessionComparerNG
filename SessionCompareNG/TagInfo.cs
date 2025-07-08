using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aveva.Core.Database;
using Aveva.Core.PMLNet;
using Aveva.Core.Database.View;
using Aveva.Core.Utilities.CommandLine;
using Aveva.Engineering.Grids.Implementation;

namespace SessionCompareNG
{
    [PMLNetCallable]
    public class TagInfo
    {
        public Db Db;
        public DbSession Session;
        public DbElementType UDET;
        public DbElement DbElement;
        public string RefNo;
        public string Name;
        public int SessionNumber;
        public bool IsNull;
        ListDefinition LstDef;
        public List<DbAttribute> PossibleAttributes = new List<DbAttribute>();
        public Dictionary<string, Attribute> AttributesDict = new Dictionary<string, Attribute>();

        [PMLNetCallable]
        public TagInfo() { }

        [PMLNetCallable]
        public TagInfo(string dbName, string udetName, string tagName, double sessionNo) 
        {
            Init(dbName, udetName, tagName, (int)sessionNo);
            ProcessAttributeValues(AttributeCollect.ALL);
        }

        [PMLNetCallable]
        public TagInfo(string dbName, string udetName, string tagName, double sessionNo, bool useLstDef)
        {
            Init(dbName, udetName, tagName, (int)sessionNo);
            LstDef = new ListDefinition("=16391/805");
            ProcessAttributeValues(AttributeCollect.LSTDEF);
        }

        public TagInfo(string dbName, string udetName, string tagName, double sessionNo, ListDefinition lstDef)
        {
            Init(dbName, udetName, tagName, (int)sessionNo);
            LstDef = lstDef;
            ProcessAttributeValues(AttributeCollect.LSTDEF);
        }

        [PMLNetCallable]
        public void Assign(TagInfo that)
        {
            Db = that.Db;
            Session = that.Session;
            UDET = that.UDET;
            DbElement = that.DbElement;
            LstDef = that.LstDef;
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
            Attribute attValue = new Attribute { Name = String.Empty, Description = String.Empty, Value = String.Empty};
            AttributesDict.TryGetValue(name.ToLower(), out attValue);
            return attValue.Value;
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

        private void Init(string dbName, string udetName, string tagName, int sessionNo)
        {
            Name = tagName;
            SessionNumber = sessionNo;
            Db = Database(dbName);
            Session = DbSession(Db, SessionNumber);
            UDET = Udet(udetName);
            PossibleAttributes = GetPossibleAttributes();
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

        private DbSession DbSession(Db db, int sessionNo)
        {
            DbSession session = db.Session(sessionNo);
            if (!session.IsValid)
            {
                throw new Exception($"Invalid Session in database {db.Name}.");
            }
            return session;
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

        private void ProcessAttributeValues(AttributeCollect ac)
        {
            Db.SwitchToOldSession(Session);
            DbElement = DbElement.GetElement(Name);

            if (!DbElement.IsNull)
            {
                IsNull = false;
                RefNo = GetReference();
                if (ac == AttributeCollect.ALL)
                {
                    AttributesDict = ProcessAttributes();
                }
                else
                {
                    AttributesDict = ProcessLstDefAttrbutes();
                }
            }
            else
            {
                IsNull = true;
            }

            if (Db.IsSwitched())
            {
                Db.SwitchBackSession(true);
            }
        }

        

        private Dictionary<string, Attribute> ProcessLstDefAttrbutes()
        {
            Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();
            attributes.Add("ref", new Attribute { Name = "RefNo", Description = "Reference Number", Value = RefNo });

            Hashtable columnDefTable = LstDef.ColDefinition;
            List<double> columnDefKeys = columnDefTable.Keys.Cast<double>().OrderBy(x => x).ToList();
            DbView dbView = LstDef.GetDbView();

            foreach (double key in columnDefKeys)
            {
                ColumnDefinition colDef = (ColumnDefinition)columnDefTable[key];
                if (!colDef.IsHidden)
                {
                    IColumn column = dbView.Columns.First(x => x.ColumnName == colDef.Key);

                    DbViewElement dbViewElement = new DbViewElement(DbElement, dbView);
                    object value = column.GetValue(dbViewElement);
                    attributes.Add(colDef.Key.ToLower(), new Attribute { Name = colDef.Key, Description = colDef.Title, Value = value.ToString() });
                }
            }

            return attributes;
        }

        private List<DbAttribute> GetPossibleAttributes()
        {
            return UDET.DisplayAttributes().ToList();
        }

        private Dictionary<string, Attribute> ProcessAttributes()
        {
            Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();

            attributes.Add("ref", new Attribute { Name = "refno", Description = "Reference Number", Value = RefNo });

            foreach (DbAttribute dbAttribute in PossibleAttributes)
            {
                string attrName = dbAttribute.Name.ToLower();
                string attrValue = DbElement.GetAsString(dbAttribute);
                string attrDesc = dbAttribute.Description;
                attributes.Add(attrName, new Attribute { Name = attrName, Description = attrDesc, Value = attrValue });
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

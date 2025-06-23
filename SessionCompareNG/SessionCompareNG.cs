using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aveva.Core.Database;
using Aveva.Core.PMLNet;

namespace SessionCompareNG
{
    public class SessionCompareNG
    {
        Dictionary<string, string> attDescriptions = new Dictionary<string, string>();
        Dictionary<string, string> baseAttributes = new Dictionary<string, string>();
        Dictionary<string, string> targetAttributes = new Dictionary<string, string>();
        List<Attribute> comparedAttributes = new List<Attribute>();

        [PMLNetCallable]
        public SessionCompareNG() { }

        [PMLNetCallable]
        public void Assign(SessionCompareNG other) { }

        [PMLNetCallable]
        public Hashtable Attributes()
        {
            return ComparedToHashtable();
        }

        [PMLNetCallable]
        public Hashtable Attribute(string attName)
        {
            Attribute compared = comparedAttributes.FirstOrDefault(a => a.Name.ToLower() == attName.ToLower());
            if (compared != null)
            {
                return compared.ToHashtable();
            }
            else
            {
                return new Hashtable();
            }
        }

        [PMLNetCallable]
        public void Print(string tag, double baseSessNo, double targetSessNo)
        {
            foreach (Attribute attCompared in comparedAttributes)
            {
                Console.WriteLine(attCompared.ToString());
            }
        }

        [PMLNetCallable]
        public void Run(string tag, double baseSessNo, double targetSessNo)
        {
            Initialize(tag, baseSessNo, targetSessNo);
        }

        private Hashtable ComparedToHashtable()
        {
            Hashtable result = new Hashtable();
            int i = 1;

            foreach (Attribute attCompared in comparedAttributes)
            {
                result[i] = attCompared.ToHashtable();
                i++;
            }

            return result;
        }

        private void Initialize(string tag, double baseSessNo, double targetSessNo)
        {
            baseAttributes = AttributesAtSession(tag, baseSessNo);
            targetAttributes = AttributesAtSession(tag, targetSessNo);
            attDescriptions = AttributeDescriptions(tag);
            comparedAttributes = Compare();
        }

        public Dictionary<string, string> AttributesAtSession(string tag, double sessno)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DbElement dbElement = DbElement.GetElement(tag);
            if (dbElement.IsNull)
            {
                return result;
            }

            Db db = dbElement.Db;
            DbSession session = db.Session((int)sessno);
            if (!session.IsValid)
            {
                return result;
            }

            db.SwitchToOldSession(session);
            dbElement = DbElement.GetElement(tag);
            DbAttribute[] attributes = dbElement.GetAttributes();
            string refno = "=" + String.Join("/", dbElement.Ref);
            result.Add("REF", refno);
            foreach (DbAttribute attr in attributes)
            {
                string attName = attr.Name;
                string attValue = dbElement.GetAsString(attr);
                result.Add(attName, attValue);
            }

            if (db.IsSwitched())
            {
                db.SwitchBackSession(true);
            }

            return result;
        }

        public Dictionary<string, string> AttributeDescriptions(string tag)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DbElement dbElement = DbElement.GetElement(tag);
            if (dbElement.IsNull)
            {
                return result;
            }

            DbAttribute[] attributes = dbElement.GetAttributes();
            result.Add("REF", "ReferenceNumber");
            foreach (DbAttribute attr in attributes)
            {
                string attName = attr.Name;
                string attDesc = attr.Description;
                result.Add(attName, attDesc);
            }

            return result;
        }

        private List<Attribute> Compare()
        {
            List<Attribute> result = new List<Attribute>();

            foreach (string key in baseAttributes.Keys)
            {
                string baseValue = baseAttributes[key];
                string targetValue = targetAttributes[key];

                if (baseValue != targetValue)
                {
                    Attribute attCompared = new Attribute
                    {
                        Name = key,
                        Description = attDescriptions[key],
                        OldValue = baseValue,
                        NewValue = targetValue
                    };

                    result.Add(attCompared);
                }
            }

            return result;
        }
    }
}

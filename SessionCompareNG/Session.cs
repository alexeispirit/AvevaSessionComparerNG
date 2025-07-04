using System;
using System.Xml;

namespace SessionCompareNG
{
    public class Session
    {
        public string User {  get; set; }
        public int Number {  get; set; }
        public DateTime DateTime { get; set; }

        public Session(string username, int number, DateTime datetime) 
        {
            User = username;
            Number = number;
            DateTime = datetime;
        }

        public void WriteXml(XmlWriter writer, SessionType stype)
        {
            writer.WriteStartElement("Session");
            writer.WriteAttributeString("Type", stype.ToString());
            writer.WriteAttributeString("Number", Number.ToString());
            writer.WriteAttributeString("User", User);
            writer.WriteAttributeString("DateTime", "yyyy-MM-dd HH:mm:ss");
            writer.WriteEndElement();
        }

        public void DebugPrint()
        {
            string datetime = DateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"Session {Number} by {User} at {datetime}");
        }
    }
}

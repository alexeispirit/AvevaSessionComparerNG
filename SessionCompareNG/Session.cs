using System;

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
    }
}

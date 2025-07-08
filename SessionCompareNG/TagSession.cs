using SessionCompareNG.Properties;

namespace SessionCompareNG
{
    public class TagSession
    {
        public string Tag;
        public int Session;
        public int PreviousSession;

        public TagSession() { }

        public TagSession(string content)
        {
            char separator = Settings.Default.ContentSeparator[0];
            string[] chunks = content.Split(separator);
            Tag = chunks[0];
            int.TryParse(chunks[1], out Session);
        }
        
        public override string ToString()
        {
            return $"{Tag}{Settings.Default.ContentSeparator}{Session}";
        }
    }
}

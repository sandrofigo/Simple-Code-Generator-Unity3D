namespace SimpleCodeGenerator.Editor
{
    public struct StringDictionaryItem
    {
        public string Key;
        public string Value;
        public string Summary;

        public StringDictionaryItem(string key, string value, string summary)
        {
            Key = key;
            Value = value;
            Summary = summary;
        }
    }
}
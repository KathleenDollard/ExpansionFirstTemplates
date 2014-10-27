namespace ExpansionFirstTemplates
{
    public struct MetadataDefinition
    {
        private string _name;
        private object _value;

        public MetadataDefinition(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public string Name { get { return _name; } }

        public object Value { get { return _value; } }

    }
}
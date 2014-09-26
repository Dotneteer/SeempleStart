namespace Seemplest.Core.UnitTests.DependencyInjection
{
    internal class SampleObject
    {
        public const string DEFAULT_STRING = "Hello";
        public const int DEFAULT_INT = 1;

        public int Property1 { get; set; }
        public string Property2 { get; set; }

        public SampleObject() : this(DEFAULT_INT, DEFAULT_STRING) { }

        public SampleObject(int prop1) : this(prop1, DEFAULT_STRING) { }

        public SampleObject(string prop2) : this(DEFAULT_INT, prop2) { }

        public SampleObject(int prop1, string property2)
        {
            Property1 = prop1;
            Property2 = property2;
        }
    }
}
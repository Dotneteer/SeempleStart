using System.IO;
using System.Xml.Linq;

namespace SeemplesTools.HtmlBuilders.UnitTests.Helpers
{
    /// <summary>
    /// This class implements a test stream to emulate the HtmlHelper's view contex writer
    /// to test Bng components
    /// </summary>
    public static class TestStream
    {
        /// <summary>
        /// The writer used for tests
        /// </summary>
        public static StringWriter StringWriter { get; private set; }

        /// <summary>
        /// Initializes the static members
        /// </summary>
        static TestStream()
        {
            Reset();
        }

        /// <summary>
        /// Resets the test stream
        /// </summary>
        public static void Reset()
        {
            StringWriter = new StringWriter();
        }

        /// <summary>
        /// The content of the test stream
        /// </summary>
        public static string Content
        {
            get { return StringWriter.ToString(); }
        }

        /// <summary>
        /// The DOM of the test stream
        /// </summary>
        public static XElement Dom
        {
            get { return XElement.Parse(StringWriter.ToString()); }
        }
    }
}
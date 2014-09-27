using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SoftwareApproach.TestingExtensions;

namespace SeemplesTools.HtmlBuilders.UnitTests.Infrastructure
{
    [TestClass]
    public class HtmlTextTest
    {
        [TestMethod]
        public void RenderWorksAsExpected()
        {
            // --- Arrange
            const string TEXT = "It's a text";
            const string EXPECTED = "It&#39;s a text";

            var text = new HtmlText(TEXT);

            // --- Act
            var rendered = text.Render();

            // --- Assert
            rendered.ShouldHaveCountOf(1);
            rendered[0].ToString().ShouldEqual(EXPECTED);
        }
    }
}
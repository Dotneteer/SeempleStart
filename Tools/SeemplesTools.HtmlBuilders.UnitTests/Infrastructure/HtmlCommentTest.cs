using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SoftwareApproach.TestingExtensions;

namespace SeemplesTools.HtmlBuilders.UnitTests.Infrastructure
{
    [TestClass]
    public class HtmlCommentTest
    {
        [TestMethod]
        public void RenderWorksAsExpected()
        {
            // --- Arrange
            const string TEXT = "It's a comment";
            const string EXPECTED = "<!-- It's a comment -->";
            var comment = new HtmlComment(TEXT);

            // --- Act
            var rendered = comment.Render();

            // --- Assert
            rendered.ShouldHaveCountOf(1);
            rendered[0].ToString().ShouldEqual(EXPECTED);
        }
    }
}

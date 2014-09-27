using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace SeemplesTools.HtmlBuilders.UnitTests.Helpers
{
    /// <summary>
    /// This class provides utility operations for testing Bng components
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Creates a new mock HtmlHelper
        /// </summary>
        /// <param name="viewData">ViewData</param>
        /// <returns>Mock HtmlHelper instance</returns>
        public static HtmlHelper CreateHtmlHelper(ViewDataDictionary viewData)
        {
            var cc = new Mock<ControllerContext>(
                new Mock<HttpContextBase>().Object,
                new RouteData(),
                new Mock<ControllerBase>().Object);

            var mockViewContext = new Mock<ViewContext>(
                cc.Object,
                new Mock<IView>().Object,
                viewData,
                new TempDataDictionary(),
                TestStream.StringWriter);

            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(viewData);
            return new HtmlHelper(mockViewContext.Object, mockViewDataContainer.Object);
        }

        /// <summary>
        /// Creates a new mock HtmlHelper with an empty model
        /// </summary>
        /// <returns>Mock HtmlHelper instance</returns>
        public static HtmlHelper CreateHtmlHelper()
        {
            return CreateHtmlHelper(new ViewDataDictionary());
        }
    }
}
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SoftwareApproach.TestingExtensions;

namespace SeemplesTools.HtmlBuilders.UnitTests.Infrastructure
{
    [TestClass]
    public class HtmlElementBaseTest
    {
        [TestMethod]
        public void ConstructionWithTagWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string EXPECTED_START = "<mytag>";
            const string EXPECTED_END = "</mytag>";

            // --- Act
            var myElement = new MyHtmlElement(TAG);

            // --- Assert
            myElement.Tag.ShouldEqual(TAG);
            myElement.HtmlAttributes.ShouldHaveCountOf(0);
            myElement.StartTag.ShouldEqual(EXPECTED_START);
            myElement.EndTag.ShouldEqual(EXPECTED_END);
        }

        [TestMethod]
        public void ConstructionWithTagAndAttributesWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string EXPECTED_START = "<mytag attr1=\"1\" attr2=\"2\" attr3=\"True\">";
            const string EXPECTED_END = "</mytag>";

            // --- Act
            var attribs = new Dictionary<string, object>
            {
                {"attr1", 1},
                {"attr2", "2"},
                {"attr3", true}
            };
            var myElement = new MyHtmlElement(TAG, attribs);

            // --- Assert
            myElement.Tag.ShouldEqual(TAG);
            myElement.StartTag.ShouldEqual(EXPECTED_START);
            myElement.EndTag.ShouldEqual(EXPECTED_END);

            myElement.HtmlAttributes.ShouldHaveCountOf(3);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr1"].ShouldEqual(1);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr2"].ShouldEqual("2");
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr3"].ShouldEqual(true);
        }

        [TestMethod]
        public void ConstructionWithTagAndAttributeParamsWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string EXPECTED_START = "<mytag attr1=\"1\" attr2=\"2\" attr3=\"True\">";
            const string EXPECTED_END = "</mytag>";

            // --- Act
            var myElement = new MyHtmlElement(TAG, "attr1", 1, "attr2", "2", "attr3", true);

            // --- Assert
            myElement.Tag.ShouldEqual(TAG);
            myElement.StartTag.ShouldEqual(EXPECTED_START);
            myElement.EndTag.ShouldEqual(EXPECTED_END);

            myElement.HtmlAttributes.ShouldHaveCountOf(3);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr1"].ShouldEqual(1);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr2"].ShouldEqual("2");
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr3"].ShouldEqual(true);
        }

        [TestMethod]
        public void ConstructionWithTagAndAttributeParamsWorksWithMissingValue()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string EXPECTED_START = "<mytag attr1=\"1\" attr2=\"2\" attr3=\"\">";
            const string EXPECTED_END = "</mytag>";

            // --- Act
            var myElement = new MyHtmlElement(TAG, "attr1", 1, "attr2", "2", "attr3");

            // --- Assert
            myElement.Tag.ShouldEqual(TAG);
            myElement.StartTag.ShouldEqual(EXPECTED_START);
            myElement.EndTag.ShouldEqual(EXPECTED_END);

            myElement.HtmlAttributes.ShouldHaveCountOf(3);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr1"].ShouldEqual(1);
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr2"].ShouldEqual("2");
            myElement.HtmlAttributes.Keys.ShouldContain("attr1");
            myElement.HtmlAttributes["attr3"].ShouldEqual("");
        }

        [TestMethod]
        public void AddCssClassWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string CLASS = "class";
            const string ATTR1 = "attr1";
            const string ATTR2 = "attr2";
            const string EXPECTED1 = ATTR1;
            const string EXPECTED2 = ATTR1 + " " + ATTR2;

            // --- Act
            var elem1 = new MyHtmlElement(TAG);
            elem1.CssClass(ATTR1);

            var elem2 = new MyHtmlElement(TAG);
            elem2.CssClass(ATTR1);
            elem2.CssClass(ATTR2);

            // --- Assert
            elem1.HtmlAttributes.ShouldHaveCountOf(1);
            elem1.HtmlAttributes.Keys.ShouldContain(CLASS);
            elem1.HtmlAttributes[CLASS].ShouldEqual(EXPECTED1);
            elem1.CssClasses.ShouldHaveCountOf(1);
            elem1.CssClasses.ShouldContain(ATTR1);

            elem2.HtmlAttributes.ShouldHaveCountOf(1);
            elem2.HtmlAttributes.Keys.ShouldContain(CLASS);
            elem2.HtmlAttributes[CLASS].ShouldEqual(EXPECTED2);
            elem2.CssClasses.ShouldHaveCountOf(2);
            elem2.CssClasses.ShouldContain(ATTR1);
            elem2.CssClasses.ShouldContain(ATTR2);
        }

        [TestMethod]
        public void AddCssClassWorksWithMultipleClasses()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string CLASS = "class";
            const string ATTR1 = "attr1";
            const string ATTR2 = "attr2";
            const string ATTR = ATTR1 + "     " + ATTR2;
            const string EXPECTED = ATTR1 + " " + ATTR2;

            // --- Act
            var elem = new MyHtmlElement(TAG);
            elem.CssClass(ATTR);

            // --- Assert
            elem.HtmlAttributes.ShouldHaveCountOf(1);
            elem.HtmlAttributes.Keys.ShouldContain(CLASS);
            elem.HtmlAttributes[CLASS].ShouldEqual(EXPECTED);
            elem.CssClasses.ShouldHaveCountOf(2);
            elem.CssClasses.ShouldContain(ATTR1);
            elem.CssClasses.ShouldContain(ATTR2);
        }

        [TestMethod]
        public void RemoveCssClassWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string CLASS = "class";
            const string ATTR1 = "attr1";
            const string ATTR2 = "attr2";
            var elem = new MyHtmlElement(TAG);
            elem.CssClass(ATTR1);
            elem.CssClass(ATTR2);

            // --- Act
            elem.RemoveCssClass(ATTR1);

            // --- Assert
            elem.HtmlAttributes.ShouldHaveCountOf(1);
            elem.HtmlAttributes.Keys.ShouldContain(CLASS);
            elem.HtmlAttributes[CLASS].ShouldEqual(ATTR2);
            elem.CssClasses.ShouldHaveCountOf(1);
            elem.CssClasses.ShouldContain(ATTR2);
        }

        [TestMethod]
        public void RemoveCssClassRemovesCssAttributeAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string ATTR1 = "attr1";
            const string ATTR2 = "attr2";
            var elem = new MyHtmlElement(TAG);
            elem.CssClass(ATTR1);
            elem.CssClass(ATTR2);

            // --- Act
            elem.RemoveCssClass(ATTR1);
            elem.RemoveCssClass(ATTR2);

            // --- Assert
            elem.CssClasses.ShouldHaveCountOf(0);
            elem.HtmlAttributes.ShouldHaveCountOf(0);
        }

        [TestMethod]
        public void EnsureHtmlAttributeWorksAsExpected()
        {
            // --- Arrange
            const string TAG = "mytag";
            const string CLASS = "class";
            const string ATTR1 = "attr1";
            const string ATTR_NAME = "attr";
            const string ATTR_VAL = "value";
            var elem = new MyHtmlElement(TAG);
            elem.CssClass(ATTR1);

            // --- Act
            elem.Attr(ATTR_NAME, ATTR_VAL);

            // --- Assert
            elem.CssClasses.ShouldHaveCountOf(1);
            elem.CssClasses.ShouldContain(ATTR1);
            elem.HtmlAttributes.ShouldHaveCountOf(2);
            elem.HtmlAttributes.Keys.ShouldContain(CLASS);
            elem.HtmlAttributes.Keys.ShouldContain(ATTR_NAME);
            elem.HtmlAttributes[ATTR_NAME].ShouldEqual(ATTR_VAL);
        }

        public class MyHtmlElement : HtmlElementBase<MyHtmlElement>
        {
            public MyHtmlElement(string tag) : base(tag)
            {
            }

            public MyHtmlElement(string tag, IDictionary<string, object> attribs) : base(tag, attribs)
            {
            }

            public MyHtmlElement(string tag, params object[] attribs) : base(tag, attribs)
            {
            }
        }
    }
}

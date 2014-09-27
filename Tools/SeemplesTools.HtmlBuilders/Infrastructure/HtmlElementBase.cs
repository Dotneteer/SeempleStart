using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This abstract class represents an HTML element that can be written to the
    /// response stream.
    /// </summary>
    public abstract class HtmlElementBase<TThis> : HtmlBuildableBase
        where TThis: HtmlElementBase<TThis>
    {
        private const string CLASS = "class";

        // --- List of CSS classes assigned to this HTML element
        private readonly List<string> _cssClasses = new List<string>();

        // --- List of nested IHtmlBuildable elements
        private readonly List<IHtmlBuildable> _nestedElements = new List<IHtmlBuildable>();

        /// <summary>
        /// The Html attributes held by this instance
        /// </summary>
        public IDictionary<string, object> HtmlAttributes { get; protected set; }
        
        /// <summary>
        /// HTML tag
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Initializes a new instance with the specified tag name
        /// </summary>
        /// <param name="tag">Tag name</param>
        protected HtmlElementBase(string tag)
        {
            HtmlAttributes = new Dictionary<string, object>();
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        protected HtmlElementBase(string tag, IDictionary<string, object> attribs)
        {
            HtmlAttributes = new Dictionary<string, object>(attribs);
            RefreshCssClasses();
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        protected HtmlElementBase(string tag, params object[] attribs)
        {
            var attribDict = new Dictionary<string, object>();
            for (var i = 0; i < attribs.Length; i += 2)
            {
                var key = attribs[i];
                var value = i < attribs.Length - 1 ? attribs[i + 1] : "";
                attribDict.Add(key.ToString(), value);
            }
            HtmlAttributes = attribDict;
            RefreshCssClasses();
            Tag = tag;
        }

        /// <summary>
        /// Gets the CSS classes associated with this class
        /// </summary>
        public IReadOnlyCollection<string> CssClasses
        {
            get { return new ReadOnlyCollection<string>(_cssClasses); }
        }

        /// <summary>
        /// Nests the specified child element into this HTML element
        /// </summary>
        /// <param name="child">Child element</param>
        /// <returns>This instance itself</returns>
        /// <remarks>
        /// Child elements are rendered in the order they are nested
        /// </remarks>
        public HtmlElementBase<TThis> AddChild(IHtmlBuildable child)
        {
            _nestedElements.Add(child);
            child.Depth = Depth + 1;
            return this;
        }

        /// <summary>
        /// Nests the specified text to this HTML element
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>This instance itself</returns>
        /// <remarks>
        /// Child elements are rendered in the order they are nested
        /// </remarks>
        public HtmlElementBase<TThis> Text(string text)
        {
            return AddChild(new HtmlText(text));
        }

        /// <summary>
        /// Retrieves the read only list of nested HTML nodes
        /// </summary>
        public IReadOnlyCollection<IHtmlBuildable> ChildNodes
        {
            get { return new ReadOnlyCollection<IHtmlBuildable>(_nestedElements);}
        }

        /// <summary>
        /// Gets the end tag
        /// </summary>
        public string EndTag
        {
            get
            {
                var segments = Complete();
                var sb = new StringBuilder();
                foreach (var segment in segments)
                {
                    sb.Append(segment);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the start tag
        /// </summary>
        public virtual string StartTag
        {
            get
            {
                var segments = Render();
                var sb = new StringBuilder();
                foreach (var segment in segments)
                {
                    sb.Append(segment);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Ensures that the specified class name is added to the definition of this instance
        /// </summary>
        /// <param name="className">Class name to ensure</param>
        /// <returns>This instance</returns>
        public HtmlElementBase<TThis> CssClass(string className)
        {
            var classes = className.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var css in classes.Where(css => !_cssClasses.Contains(css)))
            {
                _cssClasses.Add(css);
            }
            MergeCssClasses();
            return this;
        }

        /// <summary>
        /// Conditionally adds the specified class name to this instance
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="className">Class name to ensure</param>
        /// <returns>This instance</returns>
        public HtmlElementBase<TThis> CssClass(bool condition, string className)
        {
            return condition ? CssClass(className) : this;
        }

        /// <summary>
        /// Ensures that the specified class is removed from the definition of this instance
        /// </summary>
        /// <param name="className">Class name to remove</param>
        public void RemoveCssClass(string className)
        {
            var classes = className.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var css in classes.Where(css => _cssClasses.Contains(css)))
            {
                _cssClasses.Remove(css);
            }
            MergeCssClasses();
        }

        /// <summary>
        /// Ensures that this instance will contain the specified attribute
        /// </summary>
        /// <param name="key">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public HtmlElementBase<TThis> Attr(string key, string value = null)
        {
            HtmlAttributes[key] = value ?? "";
            RefreshCssClasses();
            return this;
        }

        /// <summary>
        /// Ensures that this instance will contain the specified attribute, 
        /// if the specified condition is satisfied
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="key">Attribute name</param>
        /// <param name="value">Attribute value</param>
        public HtmlElementBase<TThis> Attr(bool condition, string key, string value = null)
        {
            return condition ? Attr(key, value) : this;
        }

        /// <summary>
        /// Adds the specified set of attributes to this instance.
        /// </summary>
        /// <param name="htmlAttributes">Attributes to add to this instance</param>
        public void SetHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            HtmlAttributes.MergeHtmlAttributes(htmlAttributes.ObjectToHtmlAttributesDictionary());
        }

        /// <summary>
        /// Adds the specified set of attributes to this instance.
        /// </summary>
        /// <param name="data">Object the properties of which define the attributes</param>
        public void SetHtmlAttributes(object data)
        {
            SetHtmlAttributes(data.ToDictionary().FormatHtmlAttributes());
        }

        /// <summary>
        /// Merges the specified attribute to the definition of this instance
        /// </summary>
        /// <param name="key">Attribute name</param>
        /// <param name="value">Attribute value</param>
        protected void MergeHtmlAttribute(string key, string value)
        {
            if (HtmlAttributes != null)
            {
                if (HtmlAttributes.ContainsKey(key))
                {
                    HtmlAttributes[key] = value;
                }
                else
                {
                    HtmlAttributes.Add(key, value);
                }
            }
            else
            {
                HtmlAttributes = new Dictionary<string, object> {{key, value}};
            }
        }

        /// <summary>
        /// Decorates the related HTML element with the specified attributes
        /// </summary>
        /// <param name="data">Anonymous object describing attributes</param>
        /// <returns>This builder element</returns>
        public TThis Decorate(object data)
        {
            SetHtmlAttributes(data);
            return (TThis)this;
        }

        /// <summary>
        /// Decorates the related HTML elements with the attributes specified
        /// </summary>
        /// <param name="htmlAttributes">Set of HTML attributes</param>
        /// <returns>This builder element</returns>
        public TThis Decorate(IDictionary<string, object> htmlAttributes)
        {
            SetHtmlAttributes(htmlAttributes);
            return (TThis)this;
        }

        /// <summary>
        /// Renders the HTML buildable object
        /// </summary>
        /// <returns>HTML encoded string</returns>
        /// <remarks>
        /// This method should render the start tag, and the nested content of the HTML buildable,
        /// optionally may render the closing tag.
        /// </remarks>
        public override List<RenderedSegment> Render()
        {
            // --- Initialize the start tag of the element
            var segments = new List<RenderedSegment>();
            if (!string.IsNullOrEmpty(Tag))
            {
                var builder = new TagBuilder(Tag);
                builder.MergeAttributes(HtmlAttributes);
                var mode = VoidElements.Contains(Tag) 
                    ? TagRenderMode.SelfClosing 
                    : TagRenderMode.StartTag;
                segments.Add(new RenderedSegment(builder.ToString(mode), Depth));
            }

            // --- Build all nested elements
            foreach (var child in _nestedElements)
            {
                segments.AddRange(child.Render());
                segments.AddRange(child.Complete());
            }

            // --- Now, we're ready
            return segments;
        }

        /// <summary>
        /// Renders the (optional) closing tag of the buildable object
        /// </summary>
        /// <returns>HTML encoded string</returns>
        /// <remarks>
        /// This tag should render the closing tag if and only if that Build() have not 
        /// already done it.
        /// </remarks>
        public override List<RenderedSegment> Complete()
        {
            if (string.IsNullOrEmpty(Tag) || VoidElements.Contains(Tag))
            {
                return new List<RenderedSegment>();
            }
            return new List<RenderedSegment>
            {
                new RenderedSegment(string.Format("</{0}>", Tag), Depth)
            };
        }

        #region Helper methods

        /// <summary>
        /// Merge the changes of CSS classes with HtmlAttributes
        /// </summary>
        private void MergeCssClasses()
        {
            if (_cssClasses.Count == 0 && HtmlAttributes.ContainsKey(CLASS))
            {
                HtmlAttributes.Remove(CLASS);
            }
            else
            {
                HtmlAttributes[CLASS] = string.Join(" ", _cssClasses);
            }
        }

        /// <summary>
        /// Refresh the changes of HtmlAttributes with CSS classes
        /// </summary>
        private void RefreshCssClasses()
        {
            object classValues;
            if (HtmlAttributes.TryGetValue(CLASS, out classValues))
            {
                CssClass(classValues.ToString());
            }
        }

        #endregion
    }
}
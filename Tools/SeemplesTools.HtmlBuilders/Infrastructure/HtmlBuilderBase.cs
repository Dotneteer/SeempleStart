using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.Mvc;

namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This base class is used to build the markup for a specific element.
    /// </summary>
    /// <typeparam name="TModel">MVC model type</typeparam>
    /// <typeparam name="TElement">HTML element to build</typeparam>
    public abstract class HtmlBuilderBase<TModel, TElement> : IHtmlBuilder, IDisposable 
        where TElement : HtmlElementBase<TElement>
    {
        // --- Builder context properties
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>
        /// This flag shows whether this builder should build the buildable immediately after 
        /// successful construction.
        /// </summary>
        protected bool BuildOnConstruction { get; private set; }

        /// <summary>
        /// The element this builder builds
        /// </summary>
        protected readonly TElement Form;

        /// <summary>
        /// The output stream
        /// </summary>
        protected readonly TextWriter TextWriter;
        
        /// <summary>
        /// The HtmlHelper used to build the markup
        /// </summary>
        protected readonly HtmlHelper<TModel> HtmlHelper;

        /// <summary>
        /// Provides the context the builder uses when building the elements
        /// </summary>
        HtmlHelper IHtmlBuilderContext.HtmlHelper
        {
            get { return HtmlHelper; }
        }

        /// <summary>
        /// This is the element that can be built by this builder.
        /// </summary>
        IHtmlBuildable IHtmlBuilder.Buildable
        {
            get { return Form; }
        }

        /// <summary>
        /// The parent builder node
        /// </summary>
        public IHtmlBuilderContext ParentBuilderContext { get; private set; }

        /// <summary>
        /// Initializes this builder instance
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to use</param>
        /// <param name="form">Element to build</param>
        /// <param name="buildOnConstruction">
        /// Shows whether this builder should build the buildable immediately after 
        /// successful construction
        /// </param>
        /// <param name="parentBuilderContext"></param>
        protected HtmlBuilderBase(HtmlHelper<TModel> htmlHelper, TElement form, 
            bool buildOnConstruction = true,
            IHtmlBuilderContext parentBuilderContext = null)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }

            HtmlHelper = htmlHelper;
            ParentBuilderContext = parentBuilderContext;
            Form = form;
            Form.SetContext(this);
            Depth = parentBuilderContext == null ? 0 : parentBuilderContext.Depth + 1;
            TextWriter = htmlHelper.ViewContext.Writer;
            BuildOnConstruction = buildOnConstruction;
            ConstructionTimeBuild();
        }

        /// <summary>
        /// Build the control at construction time, if required so
        /// </summary>
        private void ConstructionTimeBuild()
        {
            if (BuildOnConstruction)
            {
                Build();
            }
        }

        /// <summary>
        /// Carries out all tasks related to the building process.
        /// </summary>
        /// <remarks>
        /// These activities should include rendering the start tag of a certain HTML element
        /// </remarks>
        public virtual void Build()
        {
            var buildable = ((IHtmlBuilder) this).Buildable;
            Render(buildable.Render());
        }

        /// <summary>
        /// Carries out all tasks related to completing the build process.
        /// </summary>
        /// <remarks>
        /// These activites may include rendering the end tag of a certain HTML element
        /// </remarks>
        public virtual void CompleteBuild()
        {
            var buildable = ((IHtmlBuilder)this).Buildable;
            Render(buildable.Complete());
        }

        /// <summary>
        /// Sets the value of the specified property
        /// </summary>
        /// <param name="propName">Property name</param>
        /// <param name="value">Property value</param>
        /// <remarks>Setting the property value to null removes the property</remarks>
        public void SetProperty(string propName, object value)
        {
            if (propName == null)
            {
                throw new ArgumentNullException("propName");
            }
            if (value == null)
            {
                _properties.Remove(propName);
            }
            else
            {
                _properties[propName] = value;
            }
        }

        /// <summary>
        /// Gets the specified property from the context of the builder
        /// </summary>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="propName">Name of the property</param>
        /// <param name="value">Property value, if found</param>
        /// <returns>True, if the property value has been found; otherwise, false</returns>
        /// <remarks>
        /// When the property cannot be found at this builder, the search goes on the 
        /// chain of parent builders. 
        /// </remarks>
        public bool GetProperty<TProp>(string propName, out TProp value)
        {
            value = default(TProp);
            object propValue;
            if (_properties.TryGetValue(propName, out propValue))
            {
                value = (TProp) propValue;
                return true;
            }
            return ParentBuilderContext != null && ParentBuilderContext.GetProperty(propName, out value);
        }

        /// <summary>
        /// The depth of the node within the node hierarchy
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Depth { get; private set; }

        
        /// <summary>
        /// Writes the specified text to the output
        /// </summary>
        /// <param name="text">Text to render</param>
        /// <param name="depth">Indentation depth</param>
        public void Render(MvcHtmlString text, int? depth = null)
        {
            Render(new RenderedSegment(text, depth.HasValue ? depth.Value : Depth));    
        }

        /// <summary>
        /// Writes the specified segment to the output
        /// </summary>
        /// <param name="segment">The rendered segment</param>
        /// <remarks>
        /// If indentation depth is not specified, the depth of the builder is used.
        /// </remarks>
        public void Render(RenderedSegment segment)
        {
            TextWriter.Write(HtmlBuildResult.FormatSegments(new List<RenderedSegment> {segment}));
        }

        /// <summary>
        /// Writes the list of specified segments to the output
        /// </summary>
        /// <param name="segments">The rendered segments</param>
        /// <remarks>
        /// If indentation depth is not specified, the depth of the builder is used.
        /// </remarks>
        public void Render(List<RenderedSegment> segments)
        {
            foreach (var segment in segments)
            {
                Render(segment);
            }
        }

        /// <summary>
        /// Renders the specified HTML buildable object
        /// </summary>
        /// <param name="buildable"></param>
        public void Render(IHtmlBuildable buildable)
        {
            Render(buildable.Render());
            Render(buildable.Complete());
        }

        /// <summary>
        /// Retrieves the start tag to be rendered by this builder
        /// </summary>
        public MvcHtmlString StartTag
        {
            get { return HtmlBuildResult.FormatSegments((((IHtmlBuilder) this).Buildable).Render()); }
        }

        /// <summary>
        /// Retrieves the end tag to be rendered by this builder
        /// </summary>
        public MvcHtmlString EndTag
        {
            get { return HtmlBuildResult.FormatSegments((((IHtmlBuilder)this).Buildable).Complete()); }
        }

        /// <summary>
        /// Places the end tag when the builder is disposed
        /// </summary>
        public virtual void Dispose()
        {
            CompleteBuild();
        }
    }
}
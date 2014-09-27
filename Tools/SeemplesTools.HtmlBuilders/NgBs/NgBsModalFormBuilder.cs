using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.NgBs
{
    /// <summary>
    /// This builder provides operations to build a modal an Angular-Bootstrap form builder 
    /// with MVC model support and client side form action handling
    /// </summary>
    /// <typeparam name="TModel">The type of the model this builder supports</typeparam>
    public class NgBsModalFormBuilder<TModel> : NgBsFormBuilder<TModel>
    {
        /// <summary>
        /// Initializes a new instance of this builder
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper instance</param>
        /// <param name="form">form instance</param>
        public NgBsModalFormBuilder(HtmlHelper<TModel> htmlHelper, NgBsForm form)
            : base(htmlHelper, form)
        {
        }

        /// <summary>
        /// Carries out all tasks related to the building process.
        /// </summary>
        /// <remarks>
        /// These activities should include rendering the start tag of a certain HTML element
        /// </remarks>
        public override void Build()
        {
        }

        /// <summary>
        /// Carries out all tasks related to completing the build process.
        /// </summary>
        /// <remarks>
        /// These activites may include rendering the end tag of a certain HTML element
        /// </remarks>
        public override void CompleteBuild()
        {
        }

        /// <summary>
        /// Creates a builder for the modal header.
        /// </summary>
        /// <returns>Builder for the modal body</returns>
        public HeaderBuilder Header(string clickAction = "cancel()")
        {
            var div = new HtmlElement(HtmlTag.Div, HtmlAttr.Class, BsClass.Modal.Header);
            var button = new HtmlElement(HtmlTag.Button, 
                HtmlAttr.Class, BsClass.Modal.CloseButton,
                AriaAttr.Hidden, "true", 
                NgTag.NgClick, clickAction);
            button.Text("\u00D7");
            div.AddChild(button);
            return new HeaderBuilder(HtmlHelper, div);
        }

        /// <summary>
        /// Creates a builder for the modal body.
        /// </summary>
        /// <returns>Builder for the modal body</returns>
        public BodyBuilder Body()
        {
            var div = new HtmlElement(HtmlTag.Div, HtmlAttr.Class, BsClass.Modal.Body);
            return new BodyBuilder(HtmlHelper, div, this);
        }

        /// <summary>
        /// Creates a builder for the modal footer.
        /// </summary>
        /// <returns>Builder for the modal body</returns>
        public FooterBuilder Footer()
        {
            var div = new HtmlElement(HtmlTag.Div, HtmlAttr.Class, BsClass.Modal.Footer);
            return new FooterBuilder(HtmlHelper, div);
        }

        /// <summary>
        /// This builder is responsible for managing the header of the modal popup
        /// </summary>
        public class HeaderBuilder : HtmlBuilderBase<TModel, HtmlElement>
        {
            /// <summary>
            /// Initializes the builder
            /// </summary>
            /// <param name="htmlHelper">HtmlHelper instance</param>
            /// <param name="div">Div element holding the modal header</param>
            internal HeaderBuilder(HtmlHelper<TModel> htmlHelper, HtmlElement div)
                : base(htmlHelper, div)
            {
            }
        }

        /// <summary>
        /// This builder is responsible for managing the body of the modal popup
        /// </summary>
        public class BodyBuilder : HtmlBuilderBase<TModel, HtmlElement>
        {
            private readonly HtmlElement _div;
            private readonly NgBsFormBuilder<TModel> _form; 

            /// <summary>
            /// Initializes the builder
            /// </summary>
            /// <param name="htmlHelper">HtmlHelper instance</param>
            /// <param name="div">Div element holding the modal body</param>
            /// <param name="form">The builder of the modal form</param>
            internal BodyBuilder(HtmlHelper<TModel> htmlHelper, HtmlElement div, NgBsFormBuilder<TModel> form)
                : base(htmlHelper, div, false)
            {
                _div = div;
                _form = form;
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                Build();
            }

            /// <summary>
            /// Carries out all tasks related to the building process.
            /// </summary>
            /// <remarks>
            /// These activities should include rendering the start tag of a certain HTML element
            /// </remarks>
            public override void Build()
            {
                Render(new MvcHtmlString(_div.StartTag));
                Render(_form.StartTag);
            }

            /// <summary>
            /// Carries out all tasks related to completing the build process.
            /// </summary>
            /// <remarks>
            /// These activites may include rendering the end tag of a certain HTML element
            /// </remarks>
            public override void CompleteBuild()
            {
                Render(_form.EndTag);
                Render(new MvcHtmlString(_div.EndTag));
            }
        }

        /// <summary>
        /// This builder is responsible for managing the footer of the modal popup
        /// </summary>
        public class FooterBuilder : HtmlBuilderBase<TModel, HtmlElement>
        {
            /// <summary>
            /// Initializes the builder
            /// </summary>
            /// <param name="htmlHelper">HtmlHelper instance</param>
            /// <param name="div">Div element holding the modal header</param>
            internal FooterBuilder(HtmlHelper<TModel> htmlHelper, HtmlElement div)
                : base(htmlHelper, div)
            {
            }
        }
    }
}
using System;
using System.Collections.Generic;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Forms;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.NgBs
{
    /// <summary>
    /// This class represents the properties of an Angular-Bootstrap form that can use an MVC model
    /// </summary>
    public class NgBsForm: BsHtmlElementBase<NgBsForm>, IBsForm
    {
        /// <summary>
        /// Label width with extra small device
        /// </summary>
        public int? LabelWidthXs { get; private set; }

        /// <summary>
        /// Label width with small device
        /// </summary>
        public int? LabelWidthSm { get; private set; }

        /// <summary>
        /// Label width with medium device
        /// </summary>
        public int? LabelWidthMd { get; private set; }

        /// <summary>
        /// Label width with large device
        /// </summary>
        public int? LabelWidthLg { get; private set; }

        /// <summary>
        /// Control width with extra small device
        /// </summary>
        public int? InputWidthXs { get; private set; }

        /// <summary>
        /// Control width with small device
        /// </summary>
        public int? InputWidthSm { get; private set; }

        /// <summary>
        /// Control with with medium device
        /// </summary>
        public int? InputWidthMd { get; private set; }

        /// <summary>
        /// Control width with large device
        /// </summary>
        public int? InputWidthLg { get; private set; }

        /// <summary>
        /// Name of the form
        /// </summary>
        public string FormName { get; private set; }

        /// <summary>
        /// Indicates whether this is a horizontal form
        /// </summary>
        public bool IsHorizontal { get; private set; }

        /// <summary>
        /// Indicates whether this is an inline form
        /// </summary>
        public bool IsInline { get; private set; }

        /// <summary>
        /// Creates a new Angular-Bootstrap form.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        public NgBsForm(string formName)
            : base(HtmlTag.Form)
        {
            InitInstance(formName);
        }

        /// <summary>
        /// Creates a new Angular-Bootstrap form.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        public NgBsForm(string formName, IDictionary<string, object> htmlAttributes)
            : base(HtmlTag.Form, htmlAttributes)
        {
            InitInstance(formName);
        }

        /// <summary>
        /// Creates a new Angular-Bootstrap form.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        public NgBsForm(string formName, object htmlAttributes)
            : base(HtmlTag.Form, htmlAttributes)
        {
            InitInstance(formName);
        }

        // --- Initializes the form instance
        private void InitInstance(string formName)
        {
            // --- Check and init form properties
            if (formName == null)
            {
                throw new ArgumentNullException("formName");
            }
            FormName = formName;
        }

        /// <summary>
        /// Sets up the HTML element after it has been attached to its builder
        /// </summary>
        protected override void OnBuilderContextAttached()
        {
            // --- Setup additional properties
            Attr(HtmlAttr.Name, FormName);
            Attr(HtmlAttr.Role, "form");
            Attr(HtmlAttr.NoValidate);
            Attr(NgTag.NgCloak);
        }

        /// <summary>
        /// Sets the form for horizontal layout
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm Horizontal()
        {
            CssClass(BsClass.Form.Horizontal);
            IsHorizontal = true;
            return this;
        }

        /// <summary>
        /// Sets the form for inline layout
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm Inline()
        {
            CssClass(BsClass.Form.Inline);
            IsInline = true;
            return this;
        }

        /// <summary>
        /// Set label width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm LabelXs(int width)
        {
            LabelWidthXs = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm LabelSm(int width)
        {
            LabelWidthSm = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with medium device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm LabelMd(int width)
        {
            LabelWidthMd = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with large device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm LabelLg(int width)
        {
            LabelWidthLg = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm InputXs(int width)
        {
            InputWidthXs = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm InputSm(int width)
        {
            InputWidthSm = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with medium device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm InputMd(int width)
        {
            InputWidthMd = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with large device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsForm InputLg(int width)
        {
            InputWidthLg = BsClass.NormalizeWidth(width);
            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Forms;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.NgBsMvc
{
    /// <summary>
    /// This class represents the properties of an Angular-Bootstrap form that can use an MVC model
    /// </summary>
    public class NgBsMvcForm: 
        HtmlElementBase<NgBsMvcForm>,
        IBsForm
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
        /// Form action name
        /// </summary>
        public string ActionName { get; private set; }

        /// <summary>
        /// Form controller name
        /// </summary>
        public string ControllerName { get; private set; }

        /// <summary>
        /// Form roote values
        /// </summary>
        public RouteValueDictionary RouteValues { get; private set; }

        /// <summary>
        /// Form method to use
        /// </summary>
        public FormMethod FormMethod { get; private set; }

        /// <summary>
        /// Indicates whether this is a horizontal form
        /// </summary>
        public bool IsHorizontal { get; private set; }

        /// <summary>
        /// Indicates whether this is an inline form
        /// </summary>
        public bool IsInline { get; private set; }

        /// <summary>
        /// Creates a new Angular-Bootstrap form with MVC model support.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="actionName">Submit action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        /// <param name="formMethod">Form method (GET/POST)</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        public NgBsMvcForm(string formName, string actionName, string controllerName, RouteValueDictionary routeValues, 
            FormMethod formMethod, 
            IDictionary<string, object> htmlAttributes)
            : base(HtmlTag.Form, htmlAttributes)
        {
            InitInstance(formName, actionName, controllerName, routeValues, formMethod);
        }

        /// <summary>
        /// Creates a new Angular-Bootstrap form with MVC model support.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="actionName">Submit action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        public NgBsMvcForm(string formName, string actionName, string controllerName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
            : base(HtmlTag.Form, htmlAttributes)
        {
            InitInstance(formName, actionName, controllerName, routeValues, FormMethod.Post);
        }

        /// <summary>
        /// Creates a new Angular-Bootstrap form with MVC model support.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="actionName">Submit action name</param>
        /// <param name="controllerName">Controller name</param>
        public NgBsMvcForm(string formName, string actionName, string controllerName)
            : base(HtmlTag.Form)
        {
            InitInstance(formName, actionName, controllerName, null, FormMethod.Post);
        }

        /// <summary>
        /// Creates a new Angular-Bootstrap form with MVC model support.
        /// </summary>
        /// <param name="formName">Name of the form</param>
        /// <param name="actionName">Submit action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="htmlAttributes">Additional HTML attributes</param>
        public NgBsMvcForm(string formName, string actionName, string controllerName, IDictionary<string, object> htmlAttributes)
            : base(HtmlTag.Form, htmlAttributes)
        {
            InitInstance(formName, actionName, controllerName, null, FormMethod.Post);
        }

        // --- Initializes the form instance
        private void InitInstance(string formName, string actionName, string controllerName, RouteValueDictionary routeValues,
            FormMethod formMethod)
        {
            // --- Check and init form properties
            if (formName == null)
            {
                throw new ArgumentNullException("formName");
            }
            if (actionName == null)
            {
                throw new ArgumentNullException("actionName");
            }
            if (formMethod != FormMethod.Get && formMethod != FormMethod.Post)
            {
                throw new ArgumentException("Only GET or POST methods are allowed.", "formMethod");
            }
            FormName = formName;
            ActionName = actionName;
            ControllerName = controllerName;
            RouteValues = routeValues;
            FormMethod = formMethod;
        }

        /// <summary>
        /// Sets up the HTML element after it has been attached to its builder
        /// </summary>
        protected override void OnBuilderContextAttached()
        {
            // --- Setup additional properties
            var formAction = UrlHelper.GenerateUrl(null, ActionName, ControllerName, RouteValues,
                ParentContext.HtmlHelper.RouteCollection,
                ParentContext.HtmlHelper.ViewContext.RequestContext, false);

            Attr(HtmlAttr.Name, FormName);
            Attr(HtmlAttr.Method, HtmlHelper.GetFormMethodString(FormMethod));
            Attr(HtmlAttr.Action, formAction);
            Attr(HtmlAttr.Role, "form");
            Attr(HtmlAttr.NoValidate);
            Attr(NgTag.NgCloak);
        }

        /// <summary>
        /// Sets the form for horizontal layout
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm Horizontal()
        {
            CssClass(BsClass.Form.Horizontal);
            IsHorizontal = true;
            return this;
        }

        /// <summary>
        /// Sets the form for inline layout
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm Inline()
        {
            CssClass(BsClass.Form.Inline);
            IsInline = true;
            return this;
        }

        /// <summary>
        /// Set label width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm LabelXs(int width)
        {
            LabelWidthXs = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm LabelSm(int width)
        {
            LabelWidthSm = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with medium device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm LabelMd(int width)
        {
            LabelWidthMd = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with large device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm LabelLg(int width)
        {
            LabelWidthLg = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set label width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm InputXs(int width)
        {
            InputWidthXs = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with extra small device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm InputSm(int width)
        {
            InputWidthSm = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with medium device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm InputMd(int width)
        {
            InputWidthMd = BsClass.NormalizeWidth(width);
            return this;
        }

        /// <summary>
        /// Set control width with large device
        /// </summary>
        /// <returns>This instance</returns>
        public NgBsMvcForm InputLg(int width)
        {
            InputWidthLg = BsClass.NormalizeWidth(width);
            return this;
        }
    }
}
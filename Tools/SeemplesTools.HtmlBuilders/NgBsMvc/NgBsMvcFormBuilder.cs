using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Forms;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.NgBsMvc
{
    /// <summary>
    /// This class implements an Angular-Bootstrap form builder with MVC model support
    /// and Mvc postback
    /// </summary>
    /// <typeparam name="TModel">The type of the model this builder supports</typeparam>
    public class NgBsMvcFormBuilder<TModel>: 
        HtmlBuilderBase<TModel, NgBsMvcForm>,
        IFormBuilder
    {
        // --- Stores the form instance
        private readonly NgBsMvcForm _form;

        // --- Helper used by this builder
        private readonly FormBuilderHelper _buildHelper;

        /// <summary>
        /// Gets the Bootstrap form instance that is about to build
        /// </summary>
        IBsForm IFormBuilder.BsForm
        {
            get { return _form; }
        }

        /// <summary>
        /// Initializes a new instance of this builder
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper instance</param>
        /// <param name="form">form instance</param>
        public NgBsMvcFormBuilder(HtmlHelper<TModel> htmlHelper, NgBsMvcForm form)
            : base(htmlHelper, form)
        {
            _form = form;
            _buildHelper = new FormBuilderHelper(this);
        }

        /// <summary>
        /// Renders an input tag for the specified model element
        /// </summary>
        /// <typeparam name="TProperty">Property type of the element</typeparam>
        /// <param name="expression">Property expression</param>
        /// <param name="inputTagType">Type of the input tag</param>
        /// <param name="autoFocus">Autofocus type</param>
        /// <param name="validate">Validation type</param>
        public MvcHtmlString InputFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            InputTagType inputTagType = InputTagType.Text,
            AutoFocus autoFocus = AutoFocus.None,
            ValidationOption validate = ValidationOption.Always)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, HtmlHelper.ViewData);
            return _form.IsHorizontal
                ? _buildHelper.BuildHorizontalInput(modelMetadata, inputTagType, autoFocus, validate)
                : _buildHelper.BuildColumnarInput(modelMetadata, inputTagType, autoFocus, validate);
        }

        /// <summary>
        /// Renders a submit button
        /// </summary>
        /// <param name="value">Button text</param>
        /// <param name="theme">Button theme</param>
        /// <param name="outerHtmlAttributes">HTML attributes of the form group</param>
        /// <param name="innerHtmlAttributes">HTML attributes of the inner div</param>
        /// <param name="buttonHtmlAttributes">HTML attributes of the button</param>
        public MvcHtmlString SubmitButton(string value, BsButtonTheme theme = BsButtonTheme.Default,
            object outerHtmlAttributes = null, 
            object innerHtmlAttributes = null, 
            object buttonHtmlAttributes = null)
        {
            // --- The form group that encapsulates the button
            var formGroup = new BsFormGroup { Depth = Depth + 1 };
            formGroup.SetHtmlAttributes(outerHtmlAttributes);
            
            // --- The div with the buttons
            var div = new BsHtmlElement(HtmlTag.Div);
            div.SetHtmlAttributes(innerHtmlAttributes);
            div.ApplyColumnWidths(null, _form.InputWidthXs, _form.InputWidthSm, _form.InputWidthMd, _form.InputWidthLg);
            div.ApplyColumnOffsets(null, _form.LabelWidthXs, _form.LabelWidthSm, _form.LabelWidthMd, _form.LabelWidthLg);
            formGroup.AddChild(div);

            // --- The button in the div
            var button = new BsSubmitButton(value, theme);
            button.SetHtmlAttributes(buttonHtmlAttributes);
            button.Attr(NgTag.NgDisabled, string.Format("{0}.$invalid", _form.FormName));
            div.AddChild(button);

            return formGroup.Markup;
        }
    }
}
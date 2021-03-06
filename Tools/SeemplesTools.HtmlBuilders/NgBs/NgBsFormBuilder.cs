﻿using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Forms;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.NgBs
{
    /// <summary>
    /// This class implements an Angular-Bootstrap form builder with MVC model support
    /// and client side form action handling
    /// </summary>
    /// <typeparam name="TModel">The type of the model this builder supports</typeparam>
    public class NgBsFormBuilder<TModel> : 
        HtmlBuilderBase<TModel, NgBsForm>,
        IFormBuilder
    {
        // --- Stores the form instance
        private readonly NgBsForm _form;

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
        public NgBsFormBuilder(HtmlHelper<TModel> htmlHelper, NgBsForm form)
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
        /// <param name="validationOption">Validation type</param>
        public MvcHtmlString InputFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            InputTagType inputTagType = InputTagType.Text,
            AutoFocus autoFocus = AutoFocus.None,
            ValidationOption validationOption = ValidationOption.Always)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, HtmlHelper.ViewData);
            return _form.IsHorizontal
                ? _buildHelper.BuildHorizontalInput(modelMetadata, inputTagType, autoFocus, validationOption)
                : _buildHelper.BuildColumnarInput(modelMetadata, inputTagType, autoFocus, validationOption);
        }

        /// <summary>
        /// Renders an input tag for the specified model element
        /// </summary>
        /// <typeparam name="TProperty">Property type of the element</typeparam>
        /// <param name="expression">Property expression</param>
        public MvcHtmlString StaticFor<TProperty>(
            Expression<Func<TModel, TProperty>> expression)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, HtmlHelper.ViewData);
            return _form.IsHorizontal
                ? _buildHelper.BuildHorizontalStatic(modelMetadata)
                : _buildHelper.BuildColumnarStatic(modelMetadata);
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

        /// <summary>
        /// Renders a cancel button for a modal dialog
        /// </summary>
        /// <param name="ngClick">ng-click attribute value (cancel() by default)</param>
        /// <param name="theme">Button theme</param>
        /// <param name="buttonText">Button text (Resources.Gen_Cancel by default)</param>
        public MvcHtmlString CancelButton(string ngClick = null, BsButtonTheme theme = BsButtonTheme.Default, string buttonText = null)
        {
            if (ngClick == null)
            {
                ngClick = "cancel()";
            }
            if (buttonText == null)
            {
                buttonText = Resources.Gen_Cancel;
            }
            var button = new BsHtmlElement(HtmlTag.Button);
            button
                .CssClass(BsClass.Button)
                .CssClass(BsClass.ButtonTheme.From(theme))
                .Attr(NgTag.NgClick, ngClick);
            button.AddChild(new HtmlText(buttonText));
            return button.Markup;
        }

        /// <summary>
        /// Renders a cancel button for a modal dialog
        /// </summary>
        /// <param name="ngClick">ng-click attribute value (cancel() by default)</param>
        /// <param name="theme">Button theme</param>
        /// <param name="buttonText">Button text (Resources.Gen_Cancel by default)</param>
        public MvcHtmlString OkButton(string ngClick = null, BsButtonTheme theme = BsButtonTheme.Success, string buttonText = null)
        {
            if (ngClick == null)
            {
                ngClick = "ok()";
            }
            if (buttonText == null)
            {
                buttonText = Resources.Gen_Ok;
            }
            var button = new BsHtmlElement(HtmlTag.Button);
            button
                .CssClass(BsClass.Button)
                .CssClass(BsClass.ButtonTheme.From(theme))
                .Attr(NgTag.NgClick, ngClick)
                .Attr(NgTag.NgDisabled, _form.FormName + ".$invalid || disableOk");
            button.AddChild(new HtmlText(buttonText));
            return button.Markup;
        }
    }
}
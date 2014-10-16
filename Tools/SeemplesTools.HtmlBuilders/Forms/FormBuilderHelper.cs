using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Infrastructure;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplesTools.HtmlBuilders.Forms
{
    /// <summary>
    /// This class provides helper functions for form builders
    /// </summary>
    public class FormBuilderHelper
    {
        // --- The builder that is supported by this helper object
        private readonly IFormBuilder _formBuilder;

        /// <summary>
        /// Creates a helper object that supports the specified builder
        /// </summary>
        /// <param name="formBuilder">Form builder to support</param>
        public FormBuilderHelper(IFormBuilder formBuilder)
        {
            _formBuilder = formBuilder;
        }

        /// <summary>
        /// Creates a text label according to the specified metadata.
        /// </summary>
        /// <param name="modelMetadata">Model metadata</param>
        /// <returns>HTML element representing the label</returns>
        public BsHtmlElement CreateTextLabel(ModelMetadata modelMetadata)
        {
            var label = new BsHtmlElement(HtmlTag.Label);
            label
                .Text(modelMetadata.DisplayName ?? modelMetadata.PropertyName)
                .CssClass(BsClass.Control.Label)
                .Attr(HtmlAttr.For, CamelCase(modelMetadata.PropertyName))
                .CssClass(modelMetadata.IsRequired, HtmlAttr.Required);
            label.ApplyColumnWidths(null, 
                _formBuilder.BsForm.LabelWidthXs, 
                _formBuilder.BsForm.LabelWidthSm, 
                _formBuilder.BsForm.LabelWidthMd, 
                _formBuilder.BsForm.LabelWidthLg);
            return label;
        }

        /// <summary>
        /// Builds a horizontal input control
        /// </summary>
        /// <param name="modelMetadata">Model metadata</param>
        /// <param name="inputTagType">Type of the input tag</param>
        /// <param name="autoFocus">Autofocus type</param>
        /// <param name="validationOption">Validation type</param>
        public MvcHtmlString BuildHorizontalInput(ModelMetadata modelMetadata, InputTagType inputTagType,
            AutoFocus autoFocus, ValidationOption validationOption)
        {
            // --- The form group that encapsulates the label and the control
            var propName = CamelCase(modelMetadata.PropertyName);
            var formGroup = new BsFormGroup {Depth = _formBuilder.Depth + 1};
            var condition = string.Format("{0}{1}",
                "{0}.{1}.$invalid",
                validationOption == ValidationOption.WhenDirty ? " && {0}.{1}.$dirty" : "");
            formGroup.Attr(NgTag.NgClass, string.Format("{{'has-error': " + condition + ", 'has-feedback': " + condition + "}}",
                _formBuilder.BsForm.FormName, propName));

            if (inputTagType == InputTagType.Text)
            {
                var label = CreateTextLabel(modelMetadata);
                var inputDiv = new BsHtmlElement(HtmlTag.Div);
                inputDiv.ApplyColumnWidths(null, 
                    _formBuilder.BsForm.InputWidthXs, 
                    _formBuilder.BsForm.InputWidthSm, 
                    _formBuilder.BsForm.InputWidthMd, 
                    _formBuilder.BsForm.InputWidthLg);
                var input = CreateInput(inputTagType, autoFocus, modelMetadata, propName);

                // --- Assemble the elements
                formGroup.AddChild(label).AddChild(inputDiv);
                inputDiv.AddChild(input);

                // --- Add optional help text
                if (!string.IsNullOrEmpty(modelMetadata.Description))
                {
                    var helpText = new BsHtmlElement(HtmlTag.Span);
                    helpText.CssClass(BsClass.Control.Help);
                    helpText.AddChild(new HtmlText(modelMetadata.Description));
                    inputDiv.AddChild(helpText);
                }

                // --- Create validation tags
                AddValidationTags(inputDiv, modelMetadata, validationOption);
            }
            else if (inputTagType == InputTagType.CheckBox)
            {
                var sizingDiv = new BsHtmlElement(HtmlTag.Div);
                sizingDiv.ApplyColumnWidths(null, 
                    _formBuilder.BsForm.InputWidthXs, 
                    _formBuilder.BsForm.InputWidthSm, 
                    _formBuilder.BsForm.InputWidthMd, 
                    _formBuilder.BsForm.InputWidthLg);
                sizingDiv.ApplyColumnOffsets(null, 
                    _formBuilder.BsForm.LabelWidthXs, 
                    _formBuilder.BsForm.LabelWidthSm, 
                    _formBuilder.BsForm.LabelWidthMd,
                    _formBuilder.BsForm.LabelWidthLg);

                var checkBoxDiv = new BsHtmlElement(HtmlTag.Div).CssClass(BsClass.Control.CheckBox);
                var label = new BsHtmlElement(HtmlTag.Label);
                var input = CreateInput(inputTagType, autoFocus, modelMetadata, propName);
                input.Attr(modelMetadata.Model != null && modelMetadata.Model.ToString().ToLower() == "true",
                    HtmlAttr.Checked, HtmlAttr.Checked);
                var hiddenInput = new BsHtmlElement(HtmlTag.Input)
                    .Attr(HtmlAttr.Name, modelMetadata.PropertyName)
                    .Attr(HtmlAttr.Type, HtmlInputType.Hidden)
                    .Attr(HtmlAttr.Value, "false");

                // --- Assemble the elements
                formGroup.AddChild(sizingDiv);
                sizingDiv.AddChild(checkBoxDiv);
                checkBoxDiv.AddChild(label);
                label
                    .AddChild(input)
                    .AddChild(new HtmlText(modelMetadata.DisplayName ?? modelMetadata.PropertyName))
                    .AddChild(hiddenInput);
            }
            return formGroup.Markup;
        }

        /// <summary>
        /// Builds a horizontal input control
        /// </summary>
        /// <param name="modelMetadata">Model metadata</param>
        /// <param name="inputTagType">Type of the input tag</param>
        /// <param name="autoFocus">Autofocus type</param>
        /// <param name="validationOption">Validation type</param>
        public MvcHtmlString BuildColumnarInput(ModelMetadata modelMetadata, InputTagType inputTagType,
            AutoFocus autoFocus, ValidationOption validationOption)
        {
            return MvcHtmlString.Empty;
        }

        public HtmlElementBase<BsHtmlElement> CreateInput(InputTagType inputTagType, AutoFocus autoFocus,
            ModelMetadata modelMetadata, string name)
        {
            name = CamelCase(name);
            var input = new BsHtmlElement(HtmlTag.Input)
                .CssClass(inputTagType != InputTagType.CheckBox, BsClass.Control.FormControl)
                .Attr(HtmlAttr.Type, GetInputTypeString(modelMetadata, inputTagType))
                .Attr(HtmlAttr.Id, name)
                .Attr(HtmlAttr.Name, name)
                .Attr(NgTag.NgModel, string.Format("model.{0}", name));
            if (modelMetadata.Model != null)
            {
                var modelValue = modelMetadata.Model.ToString();
                input
                    .Attr(HtmlAttr.Value, modelValue)
                    .Attr(NgTag.NgInit,
                        string.Format("model.{0}={1}", name, JsonConvert.SerializeObject(modelMetadata.Model)));
            }

            // --- Set the appropriate autofocus
            if (autoFocus == AutoFocus.Set
                || (_formBuilder.HtmlHelper.ViewData.ModelState.IsValid && autoFocus == AutoFocus.OnFormValid)
                || (!_formBuilder.HtmlHelper.ViewData.ModelState.IsValid && autoFocus == AutoFocus.OnFormInvalid))
            {
                input.Attr(HtmlAttr.AutoFocus);
            }

            // --- Validation attributes for input
            foreach (var attribute in modelMetadata.AdditionalValues.Values.OfType<ValidationAttributeMetadata>())
            {
                foreach (var directive in attribute.DirectiveSet)
                {
                    input.Attr(directive.Key, directive.Value ?? "");
                }
                if (attribute.AdditionalAttributes == null) continue;
                foreach (var key in attribute.AdditionalAttributes.Keys)
                {
                    input.Attr(key, attribute.AdditionalAttributes[key]);
                }
            }
            return input;
        }

        /// <summary>
        /// Renders validation tags
        /// </summary>
        /// <param name="container">Container to hold validation tags</param>
        /// <param name="modelMetadata">Model metadata</param>
        /// <param name="validationOption">Validation mode</param>
        private void AddValidationTags(HtmlElementBase<BsHtmlElement> container, ModelMetadata modelMetadata, ValidationOption validationOption)
        {
            foreach (var key in modelMetadata.AdditionalValues.Keys)
            {
                var attr = modelMetadata.AdditionalValues[key] as ValidationAttributeMetadata;
                if (attr == null) continue;
                var valTag = new HtmlElement("span")
                    .CssClass(BsClass.GlyphFa.ExclamationCircle)
                    .CssClass(BsClass.Control.Feedback)
                    .Attr(HtmlAttr.Style, "cursor: default;")
                    .Attr(BsClass.Control.Toggle, "errorhint")
                    .Attr(NgTag.NgShow, string.Format(
                        validationOption == ValidationOption.Always
                            ? "{0}.{1}.$error.{2}"
                            : "{0}.{1}.$error.{2} && {0}.{1}.$dirty",
                        _formBuilder.BsForm.FormName, CamelCase(modelMetadata.PropertyName), key))
                    .Attr(BsTag.Tooltip, string.Format(attr.ErrorMessage, modelMetadata.DisplayName))
                    .Attr(BsTag.TooltipAppendToBody, "true")
                    .Attr(BsTag.TooltipPlacement, "left");
                container.AddChild(valTag);
            }
        }

        /// <summary>
        /// Gets the type of the input string
        /// </summary>
        /// <param name="modelMetadata">Model metadata</param>
        /// <param name="inputType">Type of the input</param>
        /// <returns></returns>
        public static string GetInputTypeString(ModelMetadata modelMetadata, InputTagType inputType)
        {
            switch (inputType)
            {
                case InputTagType.CheckBox:
                    return "checkbox";
                case InputTagType.Text:
                    switch (modelMetadata.DataTypeName)
                    {
                        case "EmailAddress":
                            return "email";
                        case "Password":
                            return "password";
                        default:
                            return "text";
                    }
                default:
                    return "text";
            }
        }

        public static string CamelCase(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

    }
}
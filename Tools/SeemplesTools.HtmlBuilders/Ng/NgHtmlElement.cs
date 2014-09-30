using System.Collections.Generic;
using SeemplesTools.HtmlBuilders.Bs;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class allows you to create a Bootstrap and Angular-aware HTML element with
    /// a tag name specified at construction time
    /// </summary>
    public class NgHtmlElement<TThis> : 
        BsHtmlElementBase<NgHtmlElement<TThis>>,
        INgAware
        where TThis: NgHtmlElement<TThis>
    {
        /// <summary>
        /// Initializes a new instance with the specified tag name
        /// </summary>
        /// <param name="tag">Tag name</param>
        public NgHtmlElement(string tag)
            : base(tag)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public NgHtmlElement(string tag, IDictionary<string, object> attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public NgHtmlElement(string tag, params object[] attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Attribute value for ng-bind
        /// </summary>
        public string NgBindValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-bind attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgBind(string value)
        {
            NgBindValue = value;
            Attr(NgTag.NgBind, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-bind-html
        /// </summary>
        public string NgBindHtmlValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-bind-html attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgBindHtml(string value)
        {
            NgBindHtmlValue = value;
            Attr(NgTag.NgBindHtml, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-bind-template
        /// </summary>
        public string NgBindTemplateValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-bind-template attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgBindTemplate(string value)
        {
            NgBindTemplateValue = value;
            Attr(NgTag.NgBindTemplate, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-blur
        /// </summary>
        public string NgBlurValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-blur attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgBlur(string value)
        {
            NgBlurValue = value;
            Attr(NgTag.NgBlur, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-change
        /// </summary>
        public string NgChangeValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-change attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgChange(string value)
        {
            NgChangeValue = value;
            Attr(NgTag.NgChange, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-checked
        /// </summary>
        public string NgCheckedValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-checked attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgChecked(string value)
        {
            NgCheckedValue = value;
            Attr(NgTag.NgChange, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-class
        /// </summary>
        public string NgClassValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-class attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgClass(string value)
        {
            NgClassValue = value;
            Attr(NgTag.NgClass, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-class-even
        /// </summary>
        public string NgClassEvenValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-class-even attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgClassEven(string value)
        {
            NgClassEvenValue = value;
            Attr(NgTag.NgClassEven, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-class-odd
        /// </summary>
        public string NgClassOddValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-class-odd attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgClassOdd(string value)
        {
            NgClassOddValue = value;
            Attr(NgTag.NgClassOdd, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-click
        /// </summary>
        public string NgClickValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-click attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgClick(string value)
        {
            NgClickValue = value;
            Attr(NgTag.NgClick, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-dblclick
        /// </summary>
        public string NgDblClickValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-dblclick attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgDblClick(string value)
        {
            NgDblClickValue = value;
            Attr(NgTag.NgDblclick, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-disabled
        /// </summary>
        public string NgDisabledValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-disabled attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgDisabled(string value)
        {
            NgDisabledValue = value;
            Attr(NgTag.NgDisabled, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-focus
        /// </summary>
        public string NgFocusValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-focus attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgFocus(string value)
        {
            NgFocusValue = value;
            Attr(NgTag.NgFocus, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-hide
        /// </summary>
        public string NgHideValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-hide attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgHide(string value)
        {
            NgHideValue = value;
            Attr(NgTag.NgHide, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-href
        /// </summary>
        public string NgHrefValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-href attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgHref(string value)
        {
            NgHrefValue = value;
            Attr(NgTag.NgHref, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-if
        /// </summary>
        public string NgIfValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-if attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgIf(string value)
        {
            NgIfValue = value;
            Attr(NgTag.NgIf, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-init
        /// </summary>
        public string NgInitValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-init attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgInit(string value)
        {
            NgInitValue = value;
            Attr(NgTag.NgInit, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-keydown
        /// </summary>
        public string NgKeyDownValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-keydown attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgKeyDown(string value)
        {
            NgKeyDownValue = value;
            Attr(NgTag.NgKeydown, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-keypress
        /// </summary>
        public string NgKeyPressValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-keypress attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgKeyPress(string value)
        {
            NgKeyPressValue = value;
            Attr(NgTag.NgKeypress, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-keyup
        /// </summary>
        public string NgKeyUpValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-keyup attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgKeyUp(string value)
        {
            NgKeyUpValue = value;
            Attr(NgTag.NgKeyup, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-list
        /// </summary>
        public string NgListValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-list attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgList(string value)
        {
            NgListValue = value;
            Attr(NgTag.NgList, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-model
        /// </summary>
        public string NgModelValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-model attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgModel(string value)
        {
            NgModelValue = value;
            Attr(NgTag.NgModel, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-model-options
        /// </summary>
        public string NgModelOptionsValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-model-options attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgModelOptions(string value)
        {
            NgModelOptionsValue = value;
            Attr(NgTag.NgModelOptions, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mousedown
        /// </summary>
        public string NgMouseDownValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mousedown attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseDown(string value)
        {
            NgMouseDownValue = value;
            Attr(NgTag.NgMouseDown, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mouseenter
        /// </summary>
        public string NgMouseEnterValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mouseenter attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseEnter(string value)
        {
            NgMouseEnterValue = value;
            Attr(NgTag.NgMouseEnter, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mouseleave
        /// </summary>
        public string NgMouseLeaveValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mouseleave attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseLeave(string value)
        {
            NgMouseLeaveValue = value;
            Attr(NgTag.NgMouseLeave, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mousemove
        /// </summary>
        public string NgMouseMoveValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mousemove attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseMove(string value)
        {
            NgMouseMoveValue = value;
            Attr(NgTag.NgMouseMove, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mouseover
        /// </summary>
        public string NgMouseOverValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mouseover attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseOver(string value)
        {
            NgMouseOverValue = value;
            Attr(NgTag.NgMouseOver, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-mouseup
        /// </summary>
        public string NgMouseUpValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-mouseup attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgMouseUp(string value)
        {
            NgMouseUpValue = value;
            Attr(NgTag.NgMouseUp, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-show
        /// </summary>
        public string NgShowValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-show attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgShow(string value)
        {
            NgShowValue = value;
            Attr(NgTag.NgShow, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-src
        /// </summary>
        public string NgSrcValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-src attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgSrc(string value)
        {
            NgSrcValue = value;
            Attr(NgTag.NgSrc, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-style
        /// </summary>
        public string NgStyleValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-style attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgStyle(string value)
        {
            NgStyleValue = value;
            Attr(NgTag.NgStyle, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-submit
        /// </summary>
        public string NgSubmitValue { get; private set; }

        /// <summary>
        /// Sets the value of the ng-submit attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgSubmit(string value)
        {
            NgSubmitValue = value;
            Attr(NgTag.NgSubmit, value);
            return (TThis)this;
        }

        /// <summary>
        /// Attribute value for ng-value
        /// </summary>
        public string NgValueValue { get; private set; }
        
        /// <summary>
        /// Sets the value of the ng-value attribute
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>This instance</returns>
        public TThis NgValue(string value)
        {
            NgValueValue = value;
            Attr(NgTag.NgValue, value);
            return (TThis)this;
        }
    }
}
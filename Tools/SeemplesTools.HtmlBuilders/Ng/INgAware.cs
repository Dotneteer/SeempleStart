namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This interface represents Angular-aware simple controls
    /// </summary>
    public interface INgAware
    {
        /// <summary>
        /// Attribute value for ng-bind
        /// </summary>
        string NgBindValue { get; }

        /// <summary>
        /// Attribute value for ng-bind-html
        /// </summary>
        string NgBindHtmlValue { get; }

        /// <summary>
        /// Attribute value for ng-bind-template
        /// </summary>
        string NgBindTemplateValue { get; }

        /// <summary>
        /// Attribute value for ng-blur
        /// </summary>
        string NgBlurValue { get; }

        /// <summary>
        /// Attribute value for ng-change
        /// </summary>
        string NgChangeValue { get; }

        /// <summary>
        /// Attribute value for ng-checked
        /// </summary>
        string NgCheckedValue { get; }

        /// <summary>
        /// Attribute value for ng-class
        /// </summary>
        string NgClassValue { get; }

        /// <summary>
        /// Attribute value for ng-class-even
        /// </summary>
        string NgClassEvenValue { get; }

        /// <summary>
        /// Attribute value for ng-class-odd
        /// </summary>
        string NgClassOddValue { get; }

        /// <summary>
        /// Attribute value for ng-click
        /// </summary>
        string NgClickValue { get; }

        /// <summary>
        /// Attribute value for ng-dblclick
        /// </summary>
        string NgDblClickValue { get; }

        /// <summary>
        /// Attribute value for ng-disabled
        /// </summary>
        string NgDisabledValue { get; }

        /// <summary>
        /// Attribute value for ng-focus
        /// </summary>
        string NgFocusValue { get; }

        /// <summary>
        /// Attribute value for ng-hide
        /// </summary>
        string NgHideValue { get; }

        /// <summary>
        /// Attribute value for ng-href
        /// </summary>
        string NgHrefValue { get; }

        /// <summary>
        /// Attribute value for ng-if
        /// </summary>
        string NgIfValue { get; }

        /// <summary>
        /// Attribute value for ng-init
        /// </summary>
        string NgInitValue { get; }

        /// <summary>
        /// Attribute value for ng-keydown
        /// </summary>
        string NgKeyDownValue { get; }

        /// <summary>
        /// Attribute value for ng-keypress
        /// </summary>
        string NgKeyPressValue { get; }

        /// <summary>
        /// Attribute value for ng-keyup
        /// </summary>
        string NgKeyUpValue { get; }

        /// <summary>
        /// Attribute value for ng-list
        /// </summary>
        string NgListValue { get; }

        /// <summary>
        /// Attribute value for ng-model
        /// </summary>
        string NgModelValue { get; }

        /// <summary>
        /// Attribute value for ng-model-options
        /// </summary>
        string NgModelOptionsValue { get; }

        /// <summary>
        /// Attribute value for ng-mousedown
        /// </summary>
        string NgMouseDownValue { get; }

        /// <summary>
        /// Attribute value for ng-mouseenter
        /// </summary>
        string NgMouseEnterValue { get; }

        /// <summary>
        /// Attribute value for ng-mouseleave
        /// </summary>
        string NgMouseLeaveValue { get; }

        /// <summary>
        /// Attribute value for ng-mousemove
        /// </summary>
        string NgMouseMoveValue { get; }

        /// <summary>
        /// Attribute value for ng-mouseover
        /// </summary>
        string NgMouseOverValue { get; }

        /// <summary>
        /// Attribute value for ng-mouseup
        /// </summary>
        string NgMouseUpValue { get; }

        /// <summary>
        /// Attribute value for ng-show
        /// </summary>
        string NgShowValue { get; }

        /// <summary>
        /// Attribute value for ng-src
        /// </summary>
        string NgSrcValue { get; }

        /// <summary>
        /// Attribute value for ng-style
        /// </summary>
        string NgStyleValue { get; }

        /// <summary>
        /// Attribute value for ng-submit
        /// </summary>
        string NgSubmitValue { get; }

        /// <summary>
        /// Attribute value for ng-value
        /// </summary>
        string NgValueValue { get; }

    }
}
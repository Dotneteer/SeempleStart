using System;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This class contains contant values for Bootstrap classes
    /// </summary>
    // ReSharper disable InconsistentNaming
    public static class BsClass
    {
        public static class Form
        {
            public const string Horizontal = "form-horizontal";
            public const string Inline = "form-inline";
            public const string Group = "form-group";
        }

        public static class Modal
        {
            public const string Header = "modal-header";
            public const string Body = "modal-body";
            public const string Footer = "modal-footer";
            public const string CloseButton = "close";
        }

        public static class Control
        {
            public const string Label = "control-label";
            public const string Feedback = "form-control-feedback";
            public const string Toggle = "data-toggle";
            public const string FormControl = "form-control";
            public const string CheckBox = "checkbox";
            public const string InputGroup = "input-group";
            public const string InputGroupAddOn = "input-group-addon";
            public const string Help = "help-block";
            public const string Static = "form-control-static";
        }

        public const string Button = "btn";

        public static class ButtonTheme
        {
            public const string Default = "btn-default";
            public const string Success = "btn-success";
            public const string Info = "btn-info";
            public const string Warning = "btn-warning";
            public const string Danger = "btn-danger";

            public static string From(BsButtonTheme theme)
            {
                return String.Format("btn-{0}", theme.ToString().ToLower());
            }
        }

        public static class PanelTheme 
        {
            public const string Default = "panel-default";
        }

        public static class Column
        {
            /// <summary>
            /// Creates a col-xs-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string Xs(int width)
            {
                return string.Format("col-xs-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-sm-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string Sm(int width)
            {
                return string.Format("col-sm-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-md-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string Md(int width)
            {
                return string.Format("col-md-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-lg-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string Lg(int width)
            {
                return string.Format("col-lg-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-xs-offset-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string XsOffset(int width)
            {
                return string.Format("col-xs-offset-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-sm-offset-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string SmOffset(int width)
            {
                return string.Format("col-sm-offset-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-md-offset-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string MdOffset(int width)
            {
                return string.Format("col-md-offset-{0}", NormalizeWidth(width));
            }

            /// <summary>
            /// Creates a col-lg-offset-{n} class according to the specified width
            /// </summary>
            /// <param name="width">Column width</param>
            /// <returns>Class value</returns>
            public static string LgOffset(int width)
            {
                return string.Format("col-lg-offset-{0}", NormalizeWidth(width));
            }
        }

        /// <summary>
        /// Provides glyps
        /// </summary>
        public static class Glyph
        {
            private const string GLYPHBASE = "glyphicon glyphicon-";
            public const string WarningSign = GLYPHBASE + "warning-sign";
            public const string PlusSign = GLYPHBASE + "plus-sign";

        }

        /// <summary>
        /// Provides glyps
        /// </summary>
        public static class GlyphFa
        {
            private const string GLYPHBASE = "fa fa-";
            public const string ExclamationCircle = GLYPHBASE + "exclamation-circle";
            public const string PlusCircle = GLYPHBASE + "plus-circle";
            public const string Search = GLYPHBASE + "search";
        }

        /// <summary>
        /// Normalizes column width values
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int NormalizeWidth(int width)
        {
            return width < 1 ? 1 : (width > 12 ? 12 : width);
        }
    }
    // ReSharper restore InconsistentNaming
}
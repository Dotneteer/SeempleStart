using System.Collections.Generic;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This abstract class represents an HTML element that support Bootstrap styles
    /// </summary>
    public class BsHtmlElementBase<TThis> : HtmlElementBase<TThis> where TThis : HtmlElementBase<TThis>
    {
        /// <summary>
        /// Initializes a new instance with the specified tag name
        /// </summary>
        /// <param name="tag">Tag name</param>
        public BsHtmlElementBase(string tag)
            : base(tag)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public BsHtmlElementBase(string tag, IDictionary<string, object> attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified tag name and extra attributes
        /// </summary>
        /// <param name="tag">Tag name</param>
        /// <param name="attribs">Attribute enumeration</param>
        public BsHtmlElementBase(string tag, params object[] attribs)
            : base(tag, attribs)
        {
        }

        /// <summary>
        /// Applies the specified column widths to this element
        /// </summary>
        /// <param name="width">Optinal BsWidth attribute</param>
        /// <param name="xs">Extra small device width</param>
        /// <param name="sm">Small device width</param>
        /// <param name="md">Medium device width</param>
        /// <param name="lg">Large device width</param>
        public BsHtmlElementBase<TThis> ApplyColumnWidths(BsWidthAttribute width, int? xs, int? sm, int? md, int? lg)
        {
            var xsPresent = width == null ? xs : width.Xs;
            if (xsPresent.HasValue)
            {
                CssClass(BsClass.Column.Xs(xsPresent.Value));
            }
            var smPresent = width == null ? sm : width.Sm;
            if (smPresent.HasValue)
            {
                CssClass(BsClass.Column.Sm(smPresent.Value));
            }
            var mdPresent = width == null ? md : width.Md;
            if (mdPresent.HasValue)
            {
                CssClass(BsClass.Column.Md(mdPresent.Value));
            }
            var lgPresent = width == null ? lg : width.Lg;
            if (lgPresent.HasValue)
            {
                CssClass(BsClass.Column.Lg(lgPresent.Value));
            }
            return this;
        }

        /// <summary>
        /// Applies the specified column offsets to this element
        /// </summary>
        /// <param name="offs">Optional BsOffset attribute</param>
        /// <param name="xs">Extra small device offset</param>
        /// <param name="sm">Small device offset</param>
        /// <param name="md">Medium device offset</param>
        /// <param name="lg">Large device offset</param>
        public BsHtmlElementBase<TThis> ApplyColumnOffsets(BsOffsetAttribute offs, int? xs, int? sm, int? md, int? lg)
        {
            var xsPresent = offs == null ? xs : offs.Xs;
            if (xsPresent.HasValue)
            {
                CssClass(BsClass.Column.XsOffset(xsPresent.Value));
            }
            var smPresent = offs == null ? sm : offs.Sm;
            if (smPresent.HasValue)
            {
                CssClass(BsClass.Column.SmOffset(smPresent.Value));
            }
            var mdPresent = offs == null ? md : offs.Md;
            if (mdPresent.HasValue)
            {
                CssClass(BsClass.Column.MdOffset(mdPresent.Value));
            }
            var lgPresent = offs == null ? lg : offs.Lg;
            if (lgPresent.HasValue)
            {
                CssClass(BsClass.Column.LgOffset(lgPresent.Value));
            }
            return this;
        }
    }
}
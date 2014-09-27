using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This class renders a Bootstrap form group element
    /// </summary>
    public class BsSubmitButton: HtmlElementBase<BsSubmitButton>
    {
        /// <summary>
        /// Initializes the group
        /// </summary>
        public BsSubmitButton(string value, BsButtonTheme theme = BsButtonTheme.Default) : base(HtmlTag.Input)
        {
            CssClass(BsClass.Button);
            CssClass(BsClass.ButtonTheme.From(theme));
            Attr(HtmlAttr.Type, HtmlInputType.Submit);
            Attr(HtmlAttr.Value, value);
        }
    }
}
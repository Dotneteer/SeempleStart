using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Bs
{
    /// <summary>
    /// This class renders a Bootstrap form group element
    /// </summary>
    public class BsFormGroup: HtmlElementBase<BsFormGroup>
    {
        /// <summary>
        /// Initializes the group
        /// </summary>
        public BsFormGroup() : base(HtmlTag.Div)
        {
            CssClass(BsClass.Form.Group);
        }
    }
}
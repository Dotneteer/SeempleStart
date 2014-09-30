using System.Web.Mvc;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    public static class NgHtmlHelper
    {
        public static MvcHtmlString NgButton<TModel>(this HtmlHelper<TModel> htmleHelper, NgButton ngButton)
        {
            // --- Create the button
            ngButton.CssClass(BsClass.Button);
            ngButton.CssClass(BsClass.ButtonTheme.From(ngButton.Theme));

            // --- Add an optional icon
            if (!string.IsNullOrEmpty(ngButton.IconName))
            {
                var iconSpan = new BsHtmlElement(HtmlTag.Span);
                iconSpan.CssClass(ngButton.IconName);
                if (ngButton.IconAlignment == BsIconAlignment.Left)
                {
                    ngButton.AddChild(iconSpan);
                    ngButton.AddChild(new HtmlText("\u00a0"));
                    ngButton.AddChild(new HtmlText(ngButton.Text));
                }
                else
                {
                    ngButton.AddChild(new HtmlText(ngButton.Text));
                    ngButton.AddChild(new HtmlText("\u00a0"));
                    ngButton.AddChild(iconSpan);
                }
            }
            else
            {
                ngButton.AddChild(new HtmlText(ngButton.Text));
            }
            return ngButton.Markup;
        }

        public static MvcHtmlString NgSearchButton<TModel>(this HtmlHelper<TModel> htmleHelper, string ngModel,
            string id = "Search", string name = "Search", string placeHolder = "Search")
        {
            // --- Create the input tag
            var inputGroup = new BsHtmlElement(HtmlTag.Div).CssClass(BsClass.Control.InputGroup);
            var input = new BsHtmlElement(HtmlTag.Input);
            input.CssClass(BsClass.Control.FormControl)
                .Attr(HtmlAttr.Id, id)
                .Attr(HtmlAttr.Name, name)
                .Attr(HtmlAttr.Placeholder, placeHolder)
                .Attr(NgTag.NgModel, ngModel);
            inputGroup.AddChild(input);
            var span = new BsHtmlElement(HtmlTag.Span).CssClass(BsClass.Control.InputGroupAddOn);
            inputGroup.AddChild(span);
            span.AddChild(new BsHtmlElement(HtmlTag.Span).CssClass(BsClass.GlyphFa.Search));
            return inputGroup.Markup;
        }
    }
}
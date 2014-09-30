using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SeemplesTools.HtmlBuilders.NgBs;
using SeemplesTools.HtmlBuilders.NgBsMvc;

namespace SeemplesTools.HtmlBuilders.Ng
{
    /// <summary>
    /// This class provides methods to create Angular-Bootstrap forms
    /// </summary>
    public static class NgBsFormHelper
    {
        /// <summary>
        /// Creates a builder that generates an Angular-Bootstrap form builder with MVC model support
        /// and Mvc postback.
        /// </summary>
        /// <typeparam name="TModel">MVC Model type</typeparam>
        /// <param name="htmlHelper">HtmlHelper object</param>
        /// <param name="form">Form definition</param>
        /// <returns>Builder object</returns>
        /// <remarks>
        /// You should apply the returned builder with the C# using pattern.
        /// </remarks>
        public static NgBsMvcFormBuilder<TModel> Begin<TModel>(this HtmlHelper<TModel> htmlHelper, NgBsMvcForm form)
        {
            return new NgBsMvcFormBuilder<TModel>(htmlHelper, form);
        }

        /// <summary>
        /// Creates a builder that generates an Angular-Bootstrap form builder with MVC model support
        /// and client side form action handling.
        /// </summary>
        /// <typeparam name="TModel">MVC Model type</typeparam>
        /// <param name="htmlHelper">HtmlHelper object</param>
        /// <param name="form">Form definition</param>
        /// <returns>Builder object</returns>
        /// <remarks>
        /// You should apply the returned builder with the C# using pattern.
        /// </remarks>
        public static NgBsFormBuilder<TModel> Begin<TModel>(this HtmlHelper<TModel> htmlHelper, NgBsForm form)
        {
            return new NgBsFormBuilder<TModel>(htmlHelper, form);
        }

        /// <summary>
        /// Creates a builder that generates an Angular-Bootstrap form builder with MVC model support
        /// and client side form action handling.
        /// </summary>
        /// <typeparam name="TModel">MVC Model type</typeparam>
        /// <param name="htmlHelper">HtmlHelper object</param>
        /// <param name="form">Form definition</param>
        /// <returns>Builder object</returns>
        /// <remarks>
        /// You should apply the returned builder with the C# using pattern.
        /// </remarks>
        public static NgBsModalFormBuilder<TModel> BeginModal<TModel>(this HtmlHelper<TModel> htmlHelper, NgBsForm form)
        {
            return new NgBsModalFormBuilder<TModel>(htmlHelper, form);
        }

        /// <summary>
        /// Creates an MVC validation summary with a dismissible alert box.
        /// </summary>
        /// <typeparam name="TModel">MVC Model type</typeparam>
        /// <param name="htmlHelper">HtmlHelper object</param>
        /// <param name="excludePropertyErrors">Should property errors be excluded?</param>
        /// <param name="message">Optional message</param>
        /// <param name="htmlAttributes">Optional HTML attributes</param>
        /// <returns>The HTML output</returns>
        public static MvcHtmlString BsDismissibleValidationSummary<TModel>(this HtmlHelper<TModel> htmlHelper,
            bool excludePropertyErrors = false, string message = "", object htmlAttributes = null)
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
            {
                return new MvcHtmlString("");
            }
            var divTag = new TagBuilder("div");
            divTag.AddCssClass("alert alert-danger alert-dismissible");
            var button = new TagBuilder("button");
            button.AddCssClass("close");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("data-dismiss", "alert");
            var spanTimes = new TagBuilder("span");
            spanTimes.MergeAttribute("aria-hidden", "true");
            spanTimes.SetInnerText("\u00D7");
            var spanClose = new TagBuilder("span");
            spanClose.AddCssClass("sr-only");
            var sb = new StringBuilder();
            sb.AppendLine(spanTimes.ToString(TagRenderMode.Normal));
            sb.AppendLine(spanClose.ToString(TagRenderMode.Normal));
            button.InnerHtml = sb.ToString();
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attrs.Add("class", "text-danger");
            var summary = htmlHelper.ValidationSummary(excludePropertyErrors, message, attrs);
            sb = new StringBuilder();
            sb.AppendLine(button.ToString(TagRenderMode.Normal));
            sb.AppendLine(summary.ToString());
            divTag.InnerHtml = sb.ToString();
            return new MvcHtmlString(divTag.ToString());
        }
    }
}
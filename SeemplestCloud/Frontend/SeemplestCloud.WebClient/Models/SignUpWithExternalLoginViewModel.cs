using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    /// <summary>
    /// This view model is used by the Register dialog with an external account
    /// </summary>
    public class SignUpWithExternalLoginViewModel
    {
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Required")]
        [NgLengthRange(3, 32, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Error")]
        [Display(Name = "General_UserName", Description ="General_UserName_Help", ResourceType = typeof(FormsData))]
        public string UserName { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Error")]
        [Display(Name = "General_Email", ResourceType = typeof(FormsData))]
        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
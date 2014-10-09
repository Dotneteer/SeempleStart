using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    /// <summary>
    /// This view model is used by the Register dialog
    /// </summary>
    public class SignUpViewModel
    {
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Required")]
        [NgLengthRange(3, 32, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Error")]
        [Display(Name = "General_UserName", Description ="General_UserName_Help", ResourceType = typeof(FormsData))]
        public string UserName { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Error")]
        [Display(Name = "General_Email", ResourceType = typeof(FormsData))]
        public string Email { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Password_Required")]
        [NgLengthRange(6, 64, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Password_Error")]
        [Display(Name = "General_Password", ResourceType = typeof(FormsData))]
        [NgMatchesWith("ConfirmPassword", ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Passwords_DoNotMatch")]
        [NgStrongPassword(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_StrongPassword")]
        public string Password { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_ConfirmPassword_Required")]
        [NgLengthRange(6, 64, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Password_Error")]
        [Display(Name = "General_ConfirmPassword", ResourceType = typeof(FormsData))]
        [NgMatchesWith("Password", ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Passwords_DoNotMatch")]
        [NgStrongPassword(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_StrongPassword")]
        public string ConfirmPassword { get; set; }
    }
}
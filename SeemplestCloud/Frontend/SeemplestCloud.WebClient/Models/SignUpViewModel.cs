using System.ComponentModel;
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
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_UserName_Required")]
        [NgLengthRange(3, 32, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_UserName_Error")]
        [Display(Name = "SignUpViewModel_UserName", Description ="SignUpViewModel_UserName_Help", ResourceType = typeof(FormsData))]
        public string UserName { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Email_Error")]
        [Display(Name = "SignUpViewModel_Email", ResourceType = typeof(FormsData))]
        public string Email { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Password_Required")]
        [NgLengthRange(6, 64, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Password_Error")]
        [DataType(DataType.Password)]
        [Display(Name = "SignUpViewModel_Password", ResourceType = typeof(FormsData))]
        [NgMatchesWith("ConfirmPassword", ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Passwords_DoNotMatch")]
        public string Password { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_ConfirmPassword_Required")]
        [NgLengthRange(6, 64, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Password_Error")]
        [DataType(DataType.Password)]
        [Display(Name = "SignUpViewModel_ConfirmPassword", ResourceType = typeof(FormsData))]
        [NgMatchesWith("Password", ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SignUpViewModel_Passwords_DoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
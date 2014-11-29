using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    public class ConfirmInvitationViewModel
    {
        public int InvitationId { get; set; }

        [Display(Name = "General_UserName", Description = "General_UserName_Help", ResourceType = typeof(FormsData))]
        public string UserName { get; set; }

        [Display(Name = "General_Email", ResourceType = typeof(FormsData))]
        public string Email { get; set; }

        public int SubscriptionId { get; set; }

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
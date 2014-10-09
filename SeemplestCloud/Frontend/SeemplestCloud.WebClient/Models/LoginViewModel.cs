using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    /// <summary>
    /// This viewmodel is used by the Login dialog.
    /// </summary>
    public class LoginViewModel
    {
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Error")]
        [Display(Name = "General_Email", ResourceType = typeof(FormsData))]
        public string Email { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Password_Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "LoginViewModel_RememberMe", ResourceType = typeof(FormsData))]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
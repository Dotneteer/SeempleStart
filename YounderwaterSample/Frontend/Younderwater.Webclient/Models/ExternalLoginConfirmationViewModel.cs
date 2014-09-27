using System.ComponentModel.DataAnnotations;
using SeemplesTools.HtmlBuilders.Ng;

namespace Younderwater.Webclient.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [NgRequired]
        [NgLengthRange(3, 32, ErrorMessage = "The user name length must be between 3 and 32.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [NgRequired]
        [NgEmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
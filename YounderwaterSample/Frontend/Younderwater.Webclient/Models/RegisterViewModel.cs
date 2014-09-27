using System.ComponentModel.DataAnnotations;
using SeemplesTools.HtmlBuilders.Ng;

namespace Younderwater.Webclient.Models
{
    /// <summary>
    /// This view model is used by the Register dialog
    /// </summary>
    public class RegisterViewModel
    {
        [NgRequired]
        [NgLengthRange(3, 32, ErrorMessage = "The user name length must be between 3 and 32.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [NgRequired]
        [NgEmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [NgRequired]
        [NgLengthRange(6, 64, ErrorMessage = "The password length must be between 6 and 64.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [NgMatchesWith("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string Password { get; set; }

        [NgRequired]
        [NgLengthRange(6, 64, ErrorMessage = "The password length must be between 6 and 64.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [NgMatchesWith("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
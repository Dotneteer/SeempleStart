using System.ComponentModel.DataAnnotations;
using SeemplesTools.HtmlBuilders.Ng;

namespace Younderwater.Webclient.Models
{
    public class SetPasswordViewModel
    {
        [NgRequired]
        [NgLengthRange(6, 64, ErrorMessage = "The password length must be between 6 and 64.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [NgMatchesWith("ConfirmPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string NewPassword { get; set; }

        [NgRequired]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [NgLengthRange(6, 64, ErrorMessage = "The password length must be between 6 and 64.")]
        [NgMatchesWith("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
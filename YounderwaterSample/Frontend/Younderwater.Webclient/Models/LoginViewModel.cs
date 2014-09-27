using System.ComponentModel.DataAnnotations;
using SeemplesTools.HtmlBuilders.Ng;

namespace Younderwater.Webclient.Models
{
    /// <summary>
    /// This viewmodel is used by the Login dialog.
    /// </summary>
    public class LoginViewModel
    {
        [NgRequired]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [NgRequired]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [NgLengthRange(6, 64, ErrorMessage = "The password length must be between 6 and 64.")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
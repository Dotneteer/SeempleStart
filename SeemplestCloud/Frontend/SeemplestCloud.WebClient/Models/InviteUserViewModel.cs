using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    /// <summary>
    /// This view model is used by the Register dialog
    /// </summary>
    public class InviteUserViewModel
    {
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Required")]
        [NgLengthRange(3, 32, ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_UserName_Error")]
        [Display(Name = "InviteUsers_InvitedUserName", Description ="InviteUsers_InvitedUserHelp", ResourceType = typeof(FormsData))]
        public string InvitedUserName { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Error")]
        [Display(Name = "General_Email", Description = "InviteUsers_InvitedEmailHelp", ResourceType = typeof(FormsData))]
        public string InvitedEmail { get; set; }
    }
}
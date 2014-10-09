using System.ComponentModel.DataAnnotations;
using SeemplestCloud.Resources;
using SeemplesTools.HtmlBuilders.Ng;

namespace SeemplestCloud.WebClient.Models
{
    /// <summary>
    /// This view model is used by the Register dialog
    /// </summary>
    public class SubscriptionPackageViewModel
    {
        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "SubscriptionPackageViewModel_SubscriptionName_Required")]
        [Display(Name = "SubscriptionPackageViewModel_SubscriptionName", ResourceType = typeof(FormsData))]
        public string SubscriptionName { get; set; }

        [NgRequired(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Required")]
        [NgEmailAddress(ErrorMessageResourceType = typeof(FormsData), ErrorMessageResourceName = "General_Email_Error")]
        [Display(Name = "General_Email", ResourceType = typeof(FormsData))]
        public string PrimaryEmail { get; set; }

        public string PackageCode { get; set; }
    }
}
using Seemplest.Core.Tasks;

namespace SeemplestBlocks.Core.Email
{
    /// <summary>
    /// This class is responsible for managing the background process of emails
    /// </summary>
    public class EmailTaskProcessor : ContinuousTaskProcessor<EmailSenderTask>
    {
    }
}
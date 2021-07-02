namespace Greenhouse.Models.Options
{
    public class SendGridOptions
    {
        /// <summary>
        /// Name of email sender (Display name).
        /// </summary>
        public string SendGridUser { get; set; }

        /// <summary>
        /// Your mail that will be shown for receiver.
        /// </summary>
        public string SendGridEmail { get; set; }

        /// <summary>
        /// API Key of your SendGrid Account to send mails.
        /// </summary>
        public string SendGridKey { get; set; }
    }
}

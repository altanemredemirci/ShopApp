using SendGrid;
using SendGrid.Helpers.Mail;

namespace ShopApp.WebUI.EmailServices
{
    public class EmailSender
    {
        private const string SendGridKey = "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";

        public static async Task Execute(string subject, string message,string email)
        {
            var client = new SendGridClient(SendGridKey);

            var from = new EmailAddress("yazilim.ucuncubinyil@hotmail.com", "Üçüncübinyıl");
            var to = new EmailAddress(email);
            var plainTextContext = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContext, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}

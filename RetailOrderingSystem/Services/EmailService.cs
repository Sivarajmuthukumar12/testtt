/*
 * Folder: Services
 * File: EmailService.cs
 * Purpose: Sends transactional emails using MailKit (SMTP).
 *          Logs every email attempt to the EmailLog table.
 * Who Calls It: AuthService, OrderService
 * Interview Tip: Fire-and-forget pattern — email failures don't break the main flow.
 */

using MailKit.Net.Smtp;
using MimeKit;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, AppDbContext context, ILogger<EmailService> logger)
        {
            _config = config;
            _context = context;
            _logger = logger;
        }

        public async Task SendRegistrationEmailAsync(string toEmail, string firstName)
        {
            var subject = "Welcome to Retail Ordering System!";
            var body = $@"
                <h2>Welcome, {firstName}!</h2>
                <p>Thank you for registering with our Retail Ordering System.</p>
                <p>You can now browse our delicious Pizza, Cold Drinks, and Bread products.</p>
                <p>Happy Shopping!</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderConfirmationEmailAsync(string toEmail, string firstName,
            int orderId, decimal finalAmount)
        {
            var subject = $"Order #{orderId} Confirmed!";
            var body = $@"
                <h2>Hi {firstName},</h2>
                <p>Your order <strong>#{orderId}</strong> has been placed successfully!</p>
                <p>Total Amount: <strong>₹{finalAmount:F2}</strong></p>
                <p>We will notify you when your order is out for delivery.</p>
                <p>Thank you for ordering!</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderDeliveredEmailAsync(string toEmail, string firstName, int orderId)
        {
            var subject = $"Order #{orderId} Delivered!";
            var body = $@"
                <h2>Hi {firstName},</h2>
                <p>Your order <strong>#{orderId}</strong> has been delivered successfully!</p>
                <p>We hope you enjoy your order. Thank you for choosing us!</p>
                <p>Please rate your experience and earn loyalty points on your next order.</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            bool isSuccess = false;
            string? errorMessage = null;

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    smtpSettings["SenderName"] ?? "Retail System",
                    smtpSettings["SenderEmail"] ?? "noreply@retail.com"));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    smtpSettings["Host"] ?? "smtp.gmail.com",
                    int.Parse(smtpSettings["Port"] ?? "587"),
                    MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    smtpSettings["Username"] ?? "",
                    smtpSettings["Password"] ?? "");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                isSuccess = true;
                _logger.LogInformation("Email sent to {email}: {subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.LogWarning("Email failed to {email}: {error}", toEmail, ex.Message);
                // Don't rethrow — email failure should not break the main operation
            }
            finally
            {
                // Always log the email attempt
                try
                {
                    _context.EmailLogs.Add(new EmailLog
                    {
                        ToEmail = toEmail,
                        Subject = subject,
                        IsSuccess = isSuccess,
                        ErrorMessage = errorMessage
                    });
                    await _context.SaveChangesAsync();
                }
                catch (Exception logEx)
                {
                    _logger.LogError("Failed to log email: {error}", logEx.Message);
                }
            }
        }
    }
}

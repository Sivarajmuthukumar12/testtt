namespace RetailOrderingSystem.Interfaces
{
    public interface IEmailService
    {
        Task SendRegistrationEmailAsync(string toEmail, string firstName);
        Task SendOrderConfirmationEmailAsync(string toEmail, string firstName, int orderId, decimal finalAmount);
        Task SendOrderDeliveredEmailAsync(string toEmail, string firstName, int orderId);
    }
}

/*
 * Folder: Models
 * File: EmailLog.cs
 * Purpose: Logs every email sent by the system (success or failure).
 * Who Uses It: AppDbContext, EmailService
 */

namespace RetailOrderingSystem.Models
{
    public class EmailLog
    {
        public int Id { get; set; }
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}

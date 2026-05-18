using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using myMusic.Domain.Interfaces;

namespace myMusic.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    // --------------------------------------------------
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _configuration["SmtpSettings:SenderName"], 
            _configuration["SmtpSettings:SenderEmail"]
        ));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        
        await smtp.ConnectAsync(
            _configuration["SmtpSettings:Host"], 
            int.Parse(_configuration["SmtpSettings:Port"]!), 
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _configuration["SmtpSettings:SenderEmail"], 
            _configuration["SmtpSettings:Password"]
        );
        
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
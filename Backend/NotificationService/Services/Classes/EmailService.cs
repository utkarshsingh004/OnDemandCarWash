using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string customerName,
            string bookingId,
            string carName,
            string registrationNumber,
            string serviceType,
            decimal amount)
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.SenderName,
                    _settings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = "Green Car Wash - Car Wash Completed";

            // HTML Email
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>

    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f6f8;
            margin: 0;
            padding: 20px;
        }}

        .container {{
            max-width: 650px;
            margin: auto;
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 3px 10px rgba(0,0,0,0.1);
        }}

        .header {{
            background-color: #198754;
            color: white;
            text-align: center;
            padding: 25px;
        }}

        .header h1 {{
            margin: 0;
            font-size: 30px;
        }}

        .content {{
            padding: 30px;
        }}

        .success {{
            background-color: #e8f7ee;
            color: #198754;
            padding: 15px;
            border-radius: 6px;
            text-align: center;
            font-weight: bold;
            font-size: 18px;
        }}

        .details {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}

        .details td {{
            padding: 12px;
            border-bottom: 1px solid #eeeeee;
        }}

        .details td:first-child {{
            font-weight: bold;
            color: #555;
        }}

        .invoice {{
            margin-top: 25px;
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 20px;
        }}

        .invoice h2 {{
            margin-top: 0;
        }}

        .total {{
            font-size: 22px;
            font-weight: bold;
            text-align: right;
            margin-top: 20px;
        }}

        .footer {{
            background-color: #f8f9fa;
            text-align: center;
            padding: 20px;
            color: #777;
            font-size: 13px;
        }}
    </style>
</head>

<body>

<div class='container'>

    <div class='header'>
        <h1>Green Car Wash</h1>
        <p>Professional Car Care Service</p>
    </div>

    <div class='content'>

        <h2>Hello Utkarsh Kumar Singh </h2>

        <div class='success'>
            ✓ Your Car Wash Has Been Completed Successfully!
        </div>

        <p>
            Thank you for choosing <b>Green Car Wash</b>.
            Your vehicle has been successfully washed and is ready.
        </p>

        <h3>Booking Details</h3>

        <table class='details'>

            <tr>
                <td>Booking ID</td>
                <td> 5C8C8F7F-C9CC-43D1-8DAF-16355BA846E0 </td>
            </tr>

            <tr>
                <td>Car</td>
                <td> BMW </td>
            </tr>

            <tr>
                <td>Registration Number</td>
                <td> 1111 </td>
            </tr>

            <tr>
                <td>Service</td>
                <td> Premium </td>
            </tr>

        </table>

        <div class='invoice'>

            <h2>🧾 Invoice</h2>

            <table class='details'>

                <tr>
                    <td>Service</td>
                    <td> Premium </td>
                </tr>

                <tr>
                    <td>Amount</td>
                    <td>₹{799:N2}</td>
                </tr>

            </table>

            <div class='total'>
                Amount Due: ₹{799:N2}
            </div>

        </div>

        <p style='margin-top:25px;'>
            Please complete the payment of
            <b>₹{799:N2}</b>.
        </p>

        <p>
            Thank you for using <b>Green Car Wash</b>.
            We look forward to serving you again!
        </p>

    </div>

    <div class='footer'>
        <p><b>Green Car Wash</b></p>
        <p>Thank you for choosing us 🚗</p>
        <p>This is an automatically generated email.</p>
    </div>

</div>

</body>
</html>";

            email.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
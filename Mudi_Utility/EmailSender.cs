using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Mudi_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly HttpClient _client;

        public EmailSender(IConfiguration configuration, IHostEnvironment environment, HttpClient client)
        {
            _configuration = configuration;
            _environment = environment;
            _client = client;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_configuration["Email:Delivery"] == "File")
            {
                if (!_environment.IsDevelopment())
                    throw new InvalidOperationException("File email delivery is only available in Development.");
                var directory = Path.Combine(_environment.ContentRootPath,
                    _configuration["Email:PickupDirectory"] ?? "App_Data/mail");
                Directory.CreateDirectory(directory);
                var header = $"<p>To: {HtmlEncoder.Default.Encode(email ?? "")}<br>Subject: {HtmlEncoder.Default.Encode(subject ?? "")}</p><hr>";
                await File.WriteAllTextAsync(Path.Combine(directory, $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.html"), header + htmlMessage);
                return;
            }

            var apiKey = _configuration["MailJet:ApiKey"];
            var secret = _configuration["MailJet:SecretKey"];
            var from = _configuration["Email:FromEmail"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(from))
                throw new InvalidOperationException("Configure MailJet keys and Email:FromEmail, or use file delivery in Development.");
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailjet.com/v3.1/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{secret}")));
            request.Content = JsonContent.Create(new
            {
                Messages = new[] { new {
                    From = new { Email = from, Name = _configuration["Email:FromName"] ?? "Mudi" },
                    To = new[] { new { Email = email } },
                    Subject = subject,
                    HTMLPart = htmlMessage
                } }
            });
            using var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        string clientId = _configuration["GoogleApiCredentials:ClientId"];
        string clientSecret = _configuration["GoogleApiCredentials:ClientSecret"];
        string user = "user"; 

        UserCredential credential;

        using (var stream = new FileStream("appsettings.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { GmailService.Scope.GmailSend },
                user,
                CancellationToken.None,
                new FileDataStore("token.json"));
        }

        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "SeuApp"
        });

        var msg = CreateMessage(toEmail, subject, body);
        var result = service.Users.Messages.Send(msg, user).Execute();

        Console.WriteLine($"Message Id: {result.Id}");
    }

    private Google.Apis.Gmail.v1.Data.Message CreateMessage(string to, string subject, string body)
    {
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.TextBody = body;

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress("Seu Nome", "seuemail@gmail.com"));
        mimeMessage.To.Add(new MailboxAddress("Destinatário", to));
        mimeMessage.Subject = subject;
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        var msg = new Google.Apis.Gmail.v1.Data.Message
        {
            Raw = Base64UrlEncode(mimeMessage.ToString())
        };

        return msg;
    }

    private string Base64UrlEncode(string input)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}

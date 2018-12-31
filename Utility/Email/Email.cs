using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;

namespace Utility
{
    /// <summary>
    ///     Send email for any purpose
    /// </summary>
    public class Email
    {
        public bool IsBodyHtml { get; set; }
        private readonly string _emailHost;
        private readonly string _emailAccountId;
        private readonly string _emailAccountPassword;
        private readonly bool _isSSL;
        private readonly int _port;

        /// <summary>
        ///     Use internal host which does not require credential(id/pass)
        /// </summary>
        /// <param name="host">email hosting server. It can be an ip address or domain name</param>
        public Email(string host)
        {
            _emailHost = host;
            IsBodyHtml = false;
        }

        /// <summary>
        ///     use sdl id/pass to send email
        /// </summary>
        public Email(string host, string id, string password, bool isSSL = false, int port = 25)
        {
            _emailHost = host;
            _emailAccountId = id;
            _emailAccountPassword = password;
            IsBodyHtml = false;
            _isSSL = isSSL;
            _port = port;
        }

        public void SendEmail(IEnumerable<MailAddress> toAddresses, string fromAddress, string subject, string body, IEnumerable<Attachment> attachments, string asyncToken = "", IEnumerable<MailAddress> ccAddresses = null)
        {
            var smtp = new SmtpClient(_emailHost)
            {
                EnableSsl = _isSSL,
                Port = _port,
                Credentials = new NetworkCredential(_emailAccountId, _emailAccountPassword),
            };

            var message = new MailMessage();
            foreach (var toAddress in toAddresses)
                message.To.Add(toAddress);

            if (ccAddresses != null)
                foreach (var ccAddress in ccAddresses)
                    message.CC.Add(ccAddress);

            message.From = new MailAddress(fromAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = IsBodyHtml;
            foreach (var attachment in attachments)
                message.Attachments.Add(attachment);

            if (string.IsNullOrEmpty(asyncToken))
                smtp.Send(message);
            else
                smtp.SendAsync(message, asyncToken);
        }

        /// <summary>
        ///     To accept toAddress as string collection
        /// </summary>
        /// <param name="toAddresses"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendEmail(StringCollection toAddresses, string fromAddress, string subject, string body)
        {
            SendEmail(toAddresses, fromAddress, subject, body, new List<Attachment>());
        }

        /// <summary>
        ///     To accept toAddress as string collection
        /// </summary>
        /// <param name="toAddresses"></param>
        /// <param name="fromAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        public void SendEmail(StringCollection toAddresses, string fromAddress, string subject, string body, IEnumerable<Attachment> attachments)
        {
            var recipients = new MailAddressCollection();
            foreach (var address in toAddresses)
                recipients.Add(new MailAddress(address));

            SendEmail(recipients, fromAddress, subject, body, attachments);
        }
    }
}
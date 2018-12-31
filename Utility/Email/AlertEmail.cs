using System.Collections.Specialized;
using System.IO;

namespace Utility
{
    /// <summary>
    ///     Send alert emails with the given property
    /// </summary>
    public class AlertEmail
    {
        private AlertEmailProperty _emailProperty;

        /// <summary>
        ///     Initiate the class with email properties
        /// </summary>
        /// <param name="emailProperty"></param>
        public AlertEmail(AlertEmailProperty emailProperty)
        {
            _emailProperty = emailProperty;
        }

        /// <summary>
        ///     Send alert with the given property
        /// </summary>
        public void SendAlertEmail()
        {
            Email email;
            // Create SMTP object
            if (_emailProperty.IsSSL)
                email = new Email(_emailProperty.SMTPhost, _emailProperty.SMTPId, _emailProperty.SMTPPassword, _emailProperty.IsSSL, _emailProperty.Port);
            else
                email = new Email(_emailProperty.SMTPhost, _emailProperty.SMTPId, _emailProperty.SMTPPassword);

            // Set variables
            var from = _emailProperty.EmailFrom;
            var to = new StringCollection();
            foreach (var item in _emailProperty.EmailTo)
                to.Add(item);
            var subject = _emailProperty.Subject;
            string body;

            // use template, if provided.
            if (!string.IsNullOrEmpty(_emailProperty.Template))
            {
                body = File.ReadAllText(_emailProperty.Template);
                body = body.Replace(_emailProperty.SubjectHolder, _emailProperty.Subject);
                if (_emailProperty.isHtml)
                    _emailProperty.Body = _emailProperty.Body.Replace("\n", "<br>");

                body = body.Replace(_emailProperty.ContentHolder, _emailProperty.Body);
            }
            else
            {
                body = _emailProperty.Body;
            }

            email.IsBodyHtml = _emailProperty.isHtml;
            email.SendEmail(to, from, subject, body);
        }

        /// <summary>
        ///     Send alert with the given property and the message overwritten by the parameter
        /// </summary>
        /// <param name="message">Email message to send. This could be a full html content or main message in a html template</param>
        public void SendAlertEmail(string message)
        {
            _emailProperty.Body = message;
            SendAlertEmail();
        }
    }
}
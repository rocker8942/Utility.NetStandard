using System.Collections.Generic;

namespace Utility
{
    public struct AlertEmailProperty
    {
        public string SMTPhost { get; set; }
        public string EmailFrom { get; set; }
        public IEnumerable<string> EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Template { get; set; }
        public string ContentHolder { get; set; }
        public string SubjectHolder { get; set; }
        public bool isHtml { get; set; }
        public AlertDistribution Distribution { get; set; }
        public bool IsSSL { get; set; }
        public int Port { get; set; }
        public string SMTPId { get;set; }
        public string SMTPPassword { get; set; }
    }
}

namespace Utility.FTP
{
    public class Ftp
    {
        protected string Url { get; set; }

        protected string FtpId { get; set; }

        protected string FtpPassword { get; set; }

        protected bool IsPassive { get; set; }

        protected string RemoteDir { get; set; }
    }
}
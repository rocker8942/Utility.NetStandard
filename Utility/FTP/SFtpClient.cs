using System;
using WinSCP;

namespace Utility.FTP
{
    public class SFtpClient : Ftp, ISFtpClient
    {
        /// <summary>
        /// Default timeout is 15 seconds
        /// </summary>
        public TimeSpan Timeout
        {
            get { return _sessionOptions.Timeout; }
            set { _sessionOptions.Timeout = value; }
        }

        /// <summary>
        /// default ftp mode is passive
        /// </summary>
        public FtpMode ftpMode
        {
            get { return _sessionOptions.FtpMode; }
            set { _sessionOptions.FtpMode = value; }
        }

        private readonly SessionOptions _sessionOptions;

        /// <summary>
        /// SFTP connection 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="protocol"></param>
        /// <param name="hostKey"></param>
        /// <param name="privateKeyPath"></param>
        /// <param name="passphrase"></param>
        public SFtpClient(string hostname, string username, string password, Protocol protocol, string hostKey = null, string privateKeyPath = null, string passphrase = null)
        {
            _sessionOptions = new SessionOptions
            {
                Protocol = protocol,
                HostName = hostname,
                UserName = username,
                Password = password
            };

            if (string.IsNullOrEmpty(hostKey))
            {
                _sessionOptions.GiveUpSecurityAndAcceptAnySshHostKey = true;
                _sessionOptions.SshPrivateKeyPath = privateKeyPath;
                _sessionOptions.PrivateKeyPassphrase = passphrase;
            }
            else
            {
                _sessionOptions.SshHostKeyFingerprint = hostKey;
                _sessionOptions.SshPrivateKeyPath = privateKeyPath;
                _sessionOptions.PrivateKeyPassphrase = passphrase;
            };
        }

        /// <summary>
        /// FTP / FTPs connection 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ftpSecure"></param>
        /// <param name="portNumber"></param>
        /// <param name="tlsHostCertificateFingerprint"></param>
        public SFtpClient(string hostname, string username, string password, FtpSecure ftpSecure, string tlsHostCertificateFingerprint = "", int portNumber = 21 )
        {
            _sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = hostname,
                UserName = username,
                Password = password,
                PortNumber = portNumber,
            };

            if (ftpSecure == FtpSecure.None)
            {
                // Do nothing regarding secure set up
            }
            else
            {
                _sessionOptions.FtpSecure = ftpSecure;
                if (string.IsNullOrWhiteSpace(tlsHostCertificateFingerprint))
                {
                    // Can turn off finger print verification
                    _sessionOptions.GiveUpSecurityAndAcceptAnyTlsHostCertificate = true;
                }
                else
                {
                    // This property is for FTPS
                    _sessionOptions.TlsHostCertificateFingerprint = tlsHostCertificateFingerprint;
                }    
            }
        }

        /// <summary>
        /// Upload file through FTP 
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath">Remote path needs to be separate with '/'</param>
        /// <param name="preserveTimestamp"></param>
        /// <returns></returns>
        public TransferOperationResult FtpUpload(string localPath, string remotePath, bool preserveTimestamp = true)
        {
            using (var session = new Session())
            {
                // use if need WinSCP log while debugging
                //session.SessionLogPath = "WinSCP.log";

                // Connect
                session.Open(_sessionOptions);

                // Upload files
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    PreserveTimestamp = preserveTimestamp
                };

                TransferOperationResult transferResult = session.PutFiles(localPath, remotePath, false, transferOptions);

                // Throw on any error
                transferResult.Check();

                return transferResult;
            }
        }
    }
}
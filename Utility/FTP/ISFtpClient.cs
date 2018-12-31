using System.Collections.Generic;
using WinSCP;

namespace Utility.FTP
{
    public interface ISFtpClient
    {
        /// <summary>
        /// Upload file through FTP 
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remotePath">Remote path needs to be separate with '/'</param>
        /// <param name="preserveTimestamp"></param>
        /// <returns></returns>
        TransferOperationResult FtpUpload(string localPath, string remotePath, bool preserveTimestamp = true);
    }
}
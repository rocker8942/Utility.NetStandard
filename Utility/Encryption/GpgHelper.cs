using System;
using System.Diagnostics;
using System.IO;

namespace Utility
{
    public class GpgHelper
    {
        /// <summary>
        ///     The path to the PGP file can be changed (if needed)
        ///     or executed
        /// </summary>
        public string PgpPath { get; set; } = @"C:\Program Files\GNU\GnuPG\gpg.exe";

        /// <summary>
        ///     The location of the PubRing and SecRing files
        /// </summary>
        public string HomeDirectory { get; set; }

        public GpgHelper()
        {
        }

        /// <summary>
        ///     Public constructor stores the home directory argument
        /// </summary>
        public GpgHelper(string homeDirectory)
        {
            HomeDirectory = homeDirectory;
        }
        
        /// <summary>
        ///     Encrypts the file
        /// </summary>
        /// <param name="passPhrase">Name of the encryption file</param>
        /// <param name="fileFrom">Source file to be encrypted</param>
        /// <param name="fileTo">Destination file (after encryption)</param>
        public bool Encrypt(string passPhrase, string fileFrom, string fileTo)
        {
            var fi = new FileInfo(fileFrom);
            if (!fi.Exists)
            {
                throw new Exception("Missing file.  Cannot find the file to encrypt.");
            }

            // Cannot encrypt a file if it already exists
            if (File.Exists(fileTo))
            {
                throw new Exception("Cannot encrypt file.  File already exists");
            }

            // Confirm the existence of the PGP software
            if (!File.Exists(PgpPath + @"\gpg.exe"))
            {
                throw new Exception("Cannot find PGP software.");
            }

            // Turn off all windows for the process
            var s = new ProcessStartInfo("cmd.exe")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = fi.DirectoryName
            };

            // Execute the process and wait for it to exit.
            // NOTE: IF THE PROCESS CRASHES, it will freeze
            bool processExited = false;

            using (Process process = Process.Start(s))
            {
                // Build the encryption arguments
                // string recipient = " -r \"" + passPhrase + "\"";
                string cmd = string.Format("echo {0} | {1}gpg --batch --passphrase-fd 0 -o {2} --cipher-algo AES256 --encrypt {3} ", passPhrase, PgpPath, fileTo, fileFrom);

                if (process != null)
                {
                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                    // todo: wait until the process finished, rather than just wait for a few seconds.
                    processExited = process.WaitForExit(3500);
                }
            }
            return processExited;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="fileFrom"></param>
        /// <param name="fileTo"></param>
        /// <returns></returns>
        public bool Dencrypt(string passPhrase, string fileFrom, string fileTo)
        {
            var fi = new FileInfo(fileFrom);
            if (!fi.Exists)
            {
                throw new Exception("Missing file.  Cannot find the file to encrypt.");
            }

            // Cannot encrypt a file if it already exists
            if (File.Exists(fileTo))
            {
                throw new Exception("Cannot encrypt file.  File already exists");
            }

            // Confirm the existence of the PGP software
            if (!File.Exists(PgpPath))
            {
                throw new Exception("Cannot find PGP software.");
            }

            // Turn off all windows for the process
            var s = new ProcessStartInfo("cmd.exe");
            s.CreateNoWindow = true;
            s.UseShellExecute = false;
            s.RedirectStandardInput = true;
            s.RedirectStandardOutput = true;
            s.RedirectStandardError = true;
            s.WorkingDirectory = fi.DirectoryName;

            // Execute the process and wait for it to exit.
            // NOTE: IF THE PROCESS CRASHES, it will freeze
            bool processExited = false;

            using (Process process = Process.Start(s))
            {
                // Build the encryption arguments
                string recipient = " -r \"" + passPhrase + "\"";
                string output = " -o \"" + fileTo + "\"";
                string decrypt = " --decrypt \"" + fileFrom + "\"";
                string homedir = " --homedir \"" + HomeDirectory + "\"";
                string cmd = "\"" + PgpPath + "\"" + homedir + recipient + output + decrypt;

                if (process != null)
                {
                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                    processExited = process.WaitForExit(3500);
                }
            }
            return processExited;
        }
    }
}
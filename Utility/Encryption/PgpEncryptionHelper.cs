using System;
using System.IO;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Utility.Encryption
{
    public static class PgpEncryptionHelper
    {
        private const int BufferSize = 0x10000; // should always be power of 2

        #region Encrypt

        /*
         * Encrypt the file.
         */

        public static void EncryptFile(string inputFile, string outputFile, string publicKeyFile, bool armor,
            bool withIntegrityCheck)
        {
            using (Stream publicKeyStream = File.OpenRead(publicKeyFile))
            {
                PgpPublicKey encKey = ReadPublicKey(publicKeyStream);

                using (var bOut = new MemoryStream())
                {
                    var comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                    PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary,
                        new FileInfo(inputFile));

                    comData.Close();
                    var cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck,
                        new SecureRandom());

                    cPk.AddMethod(encKey);
                    byte[] bytes = bOut.ToArray();

                    using (Stream outputStream = File.Create(outputFile))
                    {
                        if (armor)
                        {
                            using (var armoredStream = new ArmoredOutputStream(outputStream))
                            {
                                using (Stream cOut = cPk.Open(armoredStream, bytes.Length))
                                {
                                    cOut.Write(bytes, 0, bytes.Length);
                                }
                            }
                        }
                        else
                        {
                            using (Stream cOut = cPk.Open(outputStream, bytes.Length))
                            {
                                cOut.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                }
            }
        }

        #endregion Encrypt

        #region Encrypt and Sign

        /*
         * Encrypt and sign the file pointed to by unencryptedFileInfo and
         */

        public static void EncryptAndSign(string inputFile, string outputFile, string publicKeyFile,
            string privateKeyFile, string passPhrase, bool armor)
        {
            var encryptionKeys = new PgpEncryptionKeys(publicKeyFile, privateKeyFile, passPhrase);

            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Input file [{inputFile}] does not exist.");

            if (!File.Exists(publicKeyFile))
                throw new FileNotFoundException($"Public Key file [{publicKeyFile}] does not exist.");

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException($"Private Key file [{privateKeyFile}] does not exist.");

            if (String.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("Invalid Pass Phrase.");

            if (encryptionKeys == null)
                throw new ArgumentNullException("Encryption Key not found.");

            using (Stream outputStream = File.Create(outputFile))
            {
                if (armor)
                    using (var armoredOutputStream = new ArmoredOutputStream(outputStream))
                    {
                        OutputEncrypted(inputFile, armoredOutputStream, encryptionKeys);
                    }
                else
                    OutputEncrypted(inputFile, outputStream, encryptionKeys);
            }
        }

        private static void OutputEncrypted(string inputFile, Stream outputStream, PgpEncryptionKeys encryptionKeys)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys))
            {
                var unencryptedFileInfo = new FileInfo(inputFile);
                using (Stream compressedOut = ChainCompressedOut(encryptedOut))
                {
                    PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))
                    {
                        using (FileStream inputFileStream = unencryptedFileInfo.OpenRead())
                        {
                            WriteOutputAndSign(compressedOut, literalOut, inputFileStream, signatureGenerator);
                            inputFileStream.Close();
                        }
                    }
                }
            }
        }

        private static void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFile,
            PgpSignatureGenerator signatureGenerator)
        {
            int length;
            var buf = new byte[BufferSize];
            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
                signatureGenerator.Update(buf, 0, length);
            }
            signatureGenerator.Generate().Encode(compressedOut);
        }

        private static Stream ChainEncryptedOut(Stream outputStream, PgpEncryptionKeys mEncryptionKeys)
        {
            PgpEncryptedDataGenerator encryptedDataGenerator;
            encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes,
                new SecureRandom());
            encryptedDataGenerator.AddMethod(mEncryptionKeys.PublicKey);
            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);
        }

        private static Stream ChainCompressedOut(Stream encryptedOut)
        {
            var compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            return compressedDataGenerator.Open(encryptedOut);
        }

        private static Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            var pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file);
        }

        private static PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut,
            PgpEncryptionKeys mEncryptionKeys)
        {
            const bool isCritical = false;
            const bool isNested = false;
            PublicKeyAlgorithmTag tag = mEncryptionKeys.SecretKey.PublicKey.Algorithm;
            var pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
            pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, mEncryptionKeys.PrivateKey);
            foreach (string userId in mEncryptionKeys.SecretKey.PublicKey.GetUserIds())
            {
                var subPacketGenerator = new PgpSignatureSubpacketGenerator();
                subPacketGenerator.SetSignerUserId(isCritical, userId);
                pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());
                // Just the first one!
                break;
            }
            pgpSignatureGenerator.GenerateOnePassVersion(isNested).Encode(compressedOut);
            return pgpSignatureGenerator;
        }

        #endregion Encrypt and Sign

        #region Decrypt

        /*
       * decrypt a given stream.
       */

        public static void Decrypt(string inputfile, string privateKeyFile, string passPhrase, string outputFile)
        {
            if (!File.Exists(inputfile))
                throw new FileNotFoundException($"Encrypted File [{inputfile}] not found.");

            if (!File.Exists(privateKeyFile))
                throw new FileNotFoundException($"Private Key File [{privateKeyFile}] not found.");

            if (String.IsNullOrEmpty(outputFile))
                throw new ArgumentNullException("Invalid Output file path.");

            using (Stream inputStream = File.OpenRead(inputfile))
            {
                using (Stream keyIn = File.OpenRead(privateKeyFile))
                {
                    Decrypt(inputStream, keyIn, passPhrase, outputFile);
                }
            }
        }

        /*
        * decrypt a given stream.
        */

        public static void Decrypt(Stream inputStream, Stream privateKeyStream, string passPhrase, string outputFile)
        {
            PgpObjectFactory pgpF;
            PgpEncryptedDataList enc;
            PgpObject o;
            PgpPrivateKey sKey = null;
            PgpPublicKeyEncryptedData pbe = null;
            PgpSecretKeyRingBundle pgpSec;

            pgpF = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
            // find secret key
            pgpSec = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));

            o = pgpF.NextPgpObject();

            if (o == null)
                throw new PgpException("The message is not properly encrypted");

            // the first object might be a PGP marker packet.
            if (o is PgpEncryptedDataList)
                enc = (PgpEncryptedDataList) o;
            else
                enc = (PgpEncryptedDataList) pgpF.NextPgpObject();

            // decrypt
            foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
            {
                sKey = FindSecretKey(pgpSec, pked.KeyId, passPhrase.ToCharArray());

                if (sKey != null)
                {
                    pbe = pked;
                    break;
                }
            }

            if (sKey == null)
                throw new ArgumentException("Secret key for message not found.");

            PgpObjectFactory plainFact;

            using (Stream clear = pbe.GetDataStream(sKey))
            {
                plainFact = new PgpObjectFactory(clear);
            }

            PgpObject message = plainFact.NextPgpObject();

            if (message is PgpCompressedData)
            {
                var cData = (PgpCompressedData) message;
                PgpObjectFactory of;

                using (Stream compDataIn = cData.GetDataStream())
                {
                    of = new PgpObjectFactory(compDataIn);
                }

                message = of.NextPgpObject();
                if (message is PgpOnePassSignatureList)
                {
                    message = of.NextPgpObject();
                    PgpLiteralData ld;
                    ld = (PgpLiteralData) message;
                    using (Stream output = File.Create(outputFile))
                    {
                        Stream unc = ld.GetInputStream();
                        Streams.PipeAll(unc, output);
                    }
                }
                else
                {
                    PgpLiteralData ld;
                    ld = (PgpLiteralData) message;
                    using (Stream output = File.Create(outputFile))
                    {
                        Stream unc = ld.GetInputStream();
                        Streams.PipeAll(unc, output);
                    }
                }
            }
            else if (message is PgpLiteralData)
            {
                var ld = (PgpLiteralData) message;

                using (Stream fOut = File.Create(outputFile))
                {
                    Stream unc = ld.GetInputStream();
                    Streams.PipeAll(unc, fOut);
                }
            }
            else if (message is PgpOnePassSignatureList)
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            else
                throw new PgpException("Message is not a simple encrypted file - type unknown.");
        }

        #endregion Decrypt

        #region Private helpers

        /*
        * A simple routine that opens a key ring file and loads the first available key suitable for encryption.
        */

        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            var pgpPub = new PgpPublicKeyRingBundle(inputStream);

            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            // iterate through the key rings.
            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                        return k;
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        /*
        * Search a secret key ring collection for a secret key corresponding to keyId if it exists.
        */

        private static PgpPrivateKey FindSecretKey(PgpSecretKeyRingBundle pgpSec, long keyId, char[] pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);

            if (pgpSecKey == null)
                return null;

            return pgpSecKey.ExtractPrivateKey(pass);
        }

        #endregion Private helpers
    }
}
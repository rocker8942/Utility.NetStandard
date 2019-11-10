using Utility.Encryption;
using Xunit;

namespace Utility.Test.Encryption
{
    public class CryptoTest
    {
        [Fact]
        public void EncryptStringAESTest()
        {
            var encrypted = Crypto.EncryptStringAES("test", "key");
            var decrypted = Crypto.DecryptStringAES(encrypted, "key");
            Assert.Equal("test", decrypted);
        }
    }
}
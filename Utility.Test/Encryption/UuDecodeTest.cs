using System.IO;
using Utility.Encryption;
using Xunit;

namespace Utility.Test.Encryption
{
    public class UuDecodeTest
    {
        [Fact]
        public void UuDecodeTestRun()
        {
            var text = File.ReadAllText("UuEncodedSample.txt");
            UuEncodingHelper.UuDecode(text, "test.jpg");
        }
    }
}
using System;
using Xunit;

namespace Utility.Test
{
    public class IOHelperTest
    {
        [Fact]
        public void GetNameWithDateTest()
        {
            var ioHelper = new IOHelper();
            var result = ioHelper.GetNameWithDate("abc");
            Assert.Equal(result, "abc_" + DateTime.Now.ToString("yyyyMMdd"));
        }
    }
}
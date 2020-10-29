using System.Linq;
using FluentAssertions;
using Xunit;

namespace Utility.Test
{
    public class StringHelperTest
    {
        [Fact]
        public void ExtractBetweenTest()
        {
            var text = StringHelper.ExtractBetween("abcdef", "b", "e");

            text.Should().Be("cd");
        }

        [Fact]
        public void ExtractBetweenAllTest()
        {
            var text = StringHelper.ExtractBetweenAll("<a>here</a><a>there</a>","<a>","</a>").ToList();

            text.Count.Should().Be(2);
            text[1].Should().Be("there");

        }
    }
}
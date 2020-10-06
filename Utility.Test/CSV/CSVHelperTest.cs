using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using Xunit;

namespace Utility.Test.CSV
{
    public class CSVHelperTests
    {
        /// <summary>
        ///     Test CSVHelper read csv file correctly
        /// </summary>
        [Fact]
        public void ReadCsvFastTest()
        {
            // arrange
            var mockIOHelper = new Mock<IIOHelper>();

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("id,email\r\nid1,test1.com\r\nid2,test2.com"));
            using (var streamReader = new StreamReader(stream))
            {
                // act
                var csv = new Utility.CSV.CsvHelper("");
                csv.ReadCsvFast(streamReader, ',', true);

                // assert
                var expected = new Collection<string> {"id", "email"};
                Assert.Equal(expected, csv.ColumnList);

                var recordExpected = new List<string>()
                {
                    "id1","test1.com"
                };
                recordExpected.Should().BeEquivalentTo(csv.Rows[0].ItemArray.Select(a => a.ToString()).ToList());
            }
        }
    }
}
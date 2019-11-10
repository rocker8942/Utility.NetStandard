using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Moq;
using Utility.CSV;
using Xunit;

namespace Utility.Test.CSV
{
    public class CSVHelperTests
    {
        /// <summary>
        /// Test CSVHelper read csv file correctly
        /// </summary>
        [Fact]
        public void ReadCsvFastTest()
        {
            // arrange
            var mockIOHelper = new Mock<IIOHelper>();

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("id,email\r\n1,id1,test1.com\r\nid2,test2.com"));
            using (var streamReader = new StreamReader(stream))
            {
                // act
                var csv = new CsvHelper("");
                csv.ReadCsvFast(streamReader, ',', true);

                // assert
                var expected = new Collection<string> {"id", "email"};
                Assert.Equal(expected, csv.ColumnList);
            }
        }
    }
}

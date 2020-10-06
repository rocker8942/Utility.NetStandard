using System.Collections.Generic;
using FluentAssertions;
using Utility.Enumerable;
using Xunit;

namespace Utility.Test.Enumerable
{
    public class EnumerableHelperTest
    {
        [Fact]
        public void ToDataTableTest()
        {
            // arrange
            IEnumerable<NameSet> names = new List<NameSet>
            {
                new NameSet
                {
                    ID = 1,
                    Name = "Joe"
                },
                new NameSet
                {
                    ID = 2,
                    Name = "Kay"
                }
            };

            // action
            var nameTable = names.ToDataTable();

            // assert
            nameTable.Columns.Count.Should().Be(2);
            nameTable.Columns[0].ColumnName.Should().Be("ID");
            nameTable.Columns[0].DataType.Should().Be(typeof(int));
            nameTable.Columns[1].ColumnName.Should().Be("Name");
            nameTable.Columns[1].DataType.Should().Be(typeof(string));

            nameTable.Rows.Count.Should().Be(2);
            nameTable.Rows[0].ItemArray[0].Should().Be(1);
            nameTable.Rows[0].ItemArray[1].Should().Be("Joe");
        }
    }

    public class NameSet
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
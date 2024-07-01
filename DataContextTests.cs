using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RawSqlEntityFrameworkTests;

public class DataContextTests(DataContextFixture fixture) : IClassFixture<DataContextFixture>
{
    public class MockDataContext(DbContextOptions<MockDataContext> options)
        : DbContext(options)
    { }

    private MockDataContext GetContext()
    {
        var options =
            new DbContextOptionsBuilder<MockDataContext>()
                .UseSqlite(fixture.Connection).Options;

        return new(options);
    }


    [Fact]
    public void SqlQueryRaw_Should_Return_Expected_Query_Data()
    {
        using var context = GetContext();

        var result =
            context.Database.SqlQueryRaw<MockEntity>(
"""
select * from MockEntities 
order by Id asc 
limit 1
"""
            )
            .ToArray();

        result.Should().ContainSingle();
        result[0].Should().BeEquivalentTo(new MockEntity(1, "Key1", "Value1"));
    }

    [Fact]
    public async Task SqlQueryRaw_Should_Return_Expected_Query_Data_Filtered_By_Parameters()
    {
        using var context = GetContext();

        var result =
            await context.Database.SqlQueryRaw<MockEntity>(
"""
select * from MockEntities 
where Key = @Key 
order by Id asc 
limit 1
""",
                new SqliteParameter(nameof(MockEntity.Key), "Key2")
            )
            .ToArrayAsync();

        result.Should().ContainSingle();
        result[0].Should().BeEquivalentTo(new MockEntity(2, "Key2", "Value2"));
    }

    [Fact]
    public async Task SqlQueryRaw_Should_Return_Expected_Query_Data_With_Linq_Operators_Applied()
    {
        using var context = GetContext();

        var result =
            await context.Database.SqlQueryRaw<MockEntity>("select * from MockEntities")
                .Where(x => x.Id > 3)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new MockEntity(4, "Key4", "Value4"));
    }
}

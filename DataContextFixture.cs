using Microsoft.Data.Sqlite;

namespace RawSqlEntityFrameworkTests;

public class DataContextFixture : IDisposable
{
    public SqliteConnection Connection { get; }
    private bool _disposedValue;

    public DataContextFixture()
    {
        Connection = new SqliteConnection("DataSource=:memory:");

        Connection.Open();

        new SqliteCommand(
"""
CREATE TABLE MockEntities (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Key TEXT NOT NULL UNIQUE,
    Value TEXT NOT NULL
);
""",
            Connection
        ).ExecuteNonQuery();

        new SqliteCommand(
"""
INSERT INTO MockEntities (Key, Value)
VALUES 
    ('Key1', 'Value1'),
    ('Key2', 'Value2'),
    ('Key3', 'Value3'),
    ('Key4', 'Value4'),
    ('Key5', 'Value5');
""",
            Connection
        ).ExecuteNonQuery();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        Connection.Close();
        Connection.Dispose();

        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
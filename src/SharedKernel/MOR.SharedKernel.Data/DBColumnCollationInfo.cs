namespace System.Data
{
    public record DBColumnCollationInfo(string TableName, string ColumnName, string? Collation)
    {
    }
}

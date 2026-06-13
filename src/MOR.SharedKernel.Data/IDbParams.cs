namespace System.Data
{
    public interface IDbParams
    {
        DbParam New(string field, DbType type, object? value = null);
        DbParam Int(string field, int? value = null);
        DbParam String(string field, string? value = null);
        DbParam Date(string field, DateTime? value = null);
        DbParam DateTime(string field, DateTime? value = null);
        DbParam Boolean(string field, bool? value = null);
        DbParam Decimal(string field, decimal? value = null);
        DbParam Double(string field, double? value = null);
        DbParam Guid(string field, Guid? value = null);
        DbParam TableValued(string field, ITableValuedParamConverter converter, object? value = null);

        T? GetOutput<T>(string field);

        string ToProviderSql(string sql);
        dynamic ToProviderParams();
        DbSqlParamTransformResult ToProviderSqlAndParams(string sql);
    }
}

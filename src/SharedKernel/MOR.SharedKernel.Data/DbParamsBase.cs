using System.Data.Common;

namespace System.Data
{
    public abstract class DbParamsBase<TParam> : IDbParams
        where TParam : DbParameter, new()
    {
        protected Dictionary<string, DbParam> SPPItems = new Dictionary<string, DbParam>(StringComparer.InvariantCultureIgnoreCase);
        protected List<TParam>? LastDynParams = default;


        public DbParam New(string field, DbType type, object? value = null)
        {
            var ret = new DbParam(field: field, value: value, type: type);

            if (ret.FieldName == null)
            {
                throw new ArgumentException();
            }

            SPPItems.Add(ret.FieldName, ret);

            return ret;
        }

        public DbParam Int(string field, int? value = null)
        {
            return New(field, DbType.Int32, value);
        }

        public DbParam String(string field, string? value = null)
        {
            return New(field, DbType.String, value);
        }

        public DbParam Date(string field, DateTime? value = null)
        {
            return New(field, DbType.Date, value);
        }

        public DbParam DateTime(string field, DateTime? value = null)
        {
            return New(field, DbType.DateTime, value);
        }

        public DbParam Boolean(string field, bool? value = null)
        {
            return New(field, DbType.Boolean, value);
        }

        public DbParam Decimal(string field, decimal? value = null)
        {
            return New(field, DbType.Decimal, value);
        }

        public DbParam Double(string field, double? value = null)
        {
            return New(field, DbType.Double, value);
        }

        public DbParam Guid(string field, Guid? value = null)
        {
            return New(field, DbType.Guid, value);
        }

        public DbParam TableValued(string field, ITableValuedParamConverter converter, object? value = null)
        {
            return New(field, DbType.Object, value).TableValued(converter);
        }


        public virtual T? GetOutput<T>(string field)
        {
            if (field.NullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(field));
            }

            // Here @ is required it seems
            field = "@" + field.Trim('@');

            if (LastDynParams == null)
            {
                LastDynParams = GenerateParametersCore();
            }

            var dbParam = LastDynParams.SingleOrDefault(t => t.ParameterName.EqualsInvariantIgnoreCase(field));

            if (dbParam == null)
            {
                throw new InvalidOperationException($"No parameter exist for '{field}'.");
            }

            var ret = (T?)dbParam.Value;
            return ret;
        }


        public virtual string ToProviderSql(string sql)
        {
            // TODO
            return sql;
        }

        public virtual dynamic ToProviderParams()
        {
            return GenerateParametersCore();
        }

        public virtual DbSqlParamTransformResult ToProviderSqlAndParams(string sql)
        {
            var p = GenerateParametersCore();
            var ret = new DbSqlParamTransformResult(sql, p);
            return ret;
        }



        protected abstract TParam NewStoreParam(string parameterName, object? value);


        private List<TParam> GenerateParametersCore()
        {
            var ret = new List<TParam>();

            foreach (var kvp in SPPItems)
            {
                var sp = default(TParam);
                var spp = kvp.Value;
                var field = "@" + spp.FieldName;

                if (spp.IsTableValued)
                {
                    throw new NotSupportedException($"Table valued data type is not supported for now by SqlDbParams. Field '{field}'.");
                }

                if (spp.IsCollection)
                {
                    throw new NotSupportedException($"Collection data type is not supported for now by SqlDbParams. Field '{field}'.");
                }

                if (spp.IsOutput && (spp.DataType == DbType.String))
                {
                    sp = NewStoreParam(field, string.Empty);

                    sp.DbType = spp.DataType;
                    sp.Direction = ParameterDirection.Output;
                    sp.Size = -1;
                }
                else
                {
                    sp = NewStoreParam(field, spp.ValueObject ?? DBNull.Value);
                    sp.DbType = spp.DataType;
                    sp.Direction = spp.IsOutput ? ParameterDirection.Output : ParameterDirection.Input;
                }

                ret.Add(sp);
            }

            LastDynParams = ret;
            return ret;
        }


        private string ConvertSqlForINOperator(string sql, string field, IEnumerable<object> collection)
        {
            field = field.TrimStart('@');
            var replaceParam = "@" + field;

            var paramNames = new List<string>();
            var counter = -1;

            foreach (var item in collection)
            {
                var paramName = $"@{field}{++counter}";
                paramNames.Add(paramName);
            }

            var expParams = string.Concat("(", string.Join(",", paramNames), ")");

            var ret = sql.Replace(replaceParam, expParams);
            return ret;
        }
    }
}

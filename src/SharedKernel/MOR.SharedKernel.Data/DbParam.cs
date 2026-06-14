namespace System.Data
{
    public sealed class DbParam
    {
        private ITableValuedParamConverter? TvpConverter;


        public DbParam(string field, object? value = null, DbType type = DbType.String, int? size = null)
        {
            Field(field);
            Value(value);
            Type(type);
            Size(size);
        }

        public string? FieldName { get; private set; }
        public object? ValueObject { get; private set; }
        public DbType DataType { get; private set; }
        public int? FieldSize { get; private set; }

        public bool IsCollection { get; private set; }
        public bool IsTableValued { get; private set; }

        public bool IsOutput { get; private set; }


        public DbParam Field(string field)
        {
            if (field.NullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(field));
            }

            FieldName = field.Trim('@');
            return this;
        }

        public DbParam Value(object? value)
        {
            ValueObject = value;

            IsCollection =
                (value != null) &&
                !(value is string) && // String is natively a colelction type
                (value is System.Collections.IEnumerable);

            return this;
        }

        public DbParam Type(DbType type)
        {
            DataType = type;
            return this;
        }

        public DbParam Size(int? size)
        {
            FieldSize = size;
            return this;
        }


        public DbParam TableValued(ITableValuedParamConverter converter)
        {
            TvpConverter = converter ?? throw new ArgumentNullException(nameof(converter));
            IsTableValued = true;

            return this;
        }

        public object? GetTableValuedParamValue()
        {
            if (TvpConverter == null)
            {
                throw new InvalidOperationException("Table value converter is null.");
            }

            if (IsTableValued)
            {
                var ret = TvpConverter.Convert(ValueObject);
                return ret;
            }

            throw new InvalidOperationException($"Parameter '{FieldName}' is not defined as a table valued parameter.");
        }


        public DbParam Out()
        {
            IsOutput = true;
            return this;
        }

        public DbParam In()
        {
            IsOutput = false;
            return this;
        }
    }
}

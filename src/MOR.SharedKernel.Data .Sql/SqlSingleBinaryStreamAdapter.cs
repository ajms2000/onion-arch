using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace System.Data.Sql
{
	public class SqlSingleBinaryStreamAdapter : DbSingleBinaryStreamAdapter<SqlConnection, SqlTransaction, SqlCommand, SqlParameter>
	{
		public SqlSingleBinaryStreamAdapter(string connectionString = null)
			: base(connectionString)
		{
		}
	}
}

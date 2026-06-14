using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
	public class DbSqlParamTransformResult
	{
		public DbSqlParamTransformResult(string sql, dynamic parameters)
		{
			Sql = sql;
			Parameters = parameters;
		}

		public readonly string Sql;
		public readonly dynamic Parameters;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace System.Data.Sql
{
	public static class SqlDbConnectionStringHelper
	{
		private const string CON_STR_PROP_VALUE_ENABLED = "enabled";
		private const string CON_STR_PROP_VALUE_DISABLED = "disabled";
		private const string CON_STR_PROP_VALUE_TRUE = "true";
		private const string CON_STR_PROP_VALUE_FALSE = "false";
		private const string CON_STR_PROP_VALUE_YES = "yes";
		private const string CON_STR_PROP_VALUE_NO = "no";

		private const string CON_STR_PROP_NAME_COL_ENCR = "Column Encryption Setting";
		private const string CON_STR_PROP_NAME_APP = "App";


		public static string BuildSqlConnectionString(DBConnectionStringProperties props)
		{
			var builder = new SqlConnectionStringBuilder();

			builder.InitialCatalog = props.InitialCatalog;
			builder.DataSource = props.DataSource;
			builder.UserID = props.Username;
			builder.Password = props.Password;
			builder.MultipleActiveResultSets = props.MultipleActiveResultSets;

			if (props.IntegratedSecuity)
			{
				builder.IntegratedSecurity = true;
			}
			else
			{
				builder.PersistSecurityInfo = true;
			}

			if (props.App.NotNullOrWhiteSpace())
			{
				builder.Add(CON_STR_PROP_NAME_APP, props.App);
			}

			if (props.EnableColumnEncryption)
			{
				builder.Add(CON_STR_PROP_NAME_COL_ENCR, CON_STR_PROP_VALUE_ENABLED);
			}

			var ret = builder.ConnectionString;
			return ret;
		}
	}
}

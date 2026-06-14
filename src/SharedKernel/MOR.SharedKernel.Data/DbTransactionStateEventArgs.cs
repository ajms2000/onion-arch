using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
	public class DbTransactionStateEventArgs
	{
		public readonly bool IsStarted;

		public DbTransactionStateEventArgs(bool isStarted)
		{
			IsStarted = isStarted;
		}
	}
}

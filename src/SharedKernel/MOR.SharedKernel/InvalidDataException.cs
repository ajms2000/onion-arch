using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class InvalidDataException : AppException
    {
        public InvalidDataException()
            : base("Invalid data.") { }
        public InvalidDataException(string message)
            : base(message) { }
        public InvalidDataException(string message, Exception innerException)
            : base(message, innerException) { }


        public static InvalidDataException New(string messageTemplate, params object[] args)
        {
            return New(default(Exception), messageTemplate, args);
        }

        public static InvalidDataException New(Exception? innerException, string messageTemplate, params object[] args)
        {
            var msg = string.Format(messageTemplate, args);

            if (innerException != null)
            {
                return new InvalidDataException(msg, innerException);
            }

            return new InvalidDataException(msg);
        }
    }
}

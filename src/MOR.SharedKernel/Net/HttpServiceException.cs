namespace System.Net
{
    public class HttpServiceException : AppException
    {
        /// <summary>
        /// Represents a base exception, which can used to report meaningfull messages to consumers/end users.
        /// </summary>
        /// <param name="message">Consumer friendly message.</param>
        /// <param name="httpStatus">Map to an HTTP status in web scenarios.</param>
        public HttpServiceException(string message, HttpStatusCode httpStatus = HttpStatusCode.InternalServerError)
            : base(message)
        {
            HttpStatus = httpStatus;
        }

        /// <summary>
        /// Represents a base exception, which can used to report meaningfull messages to consumers/end users.
        /// </summary>
        /// <param name="message">Consumer friendly message.</param>
        /// <param name="inner">Represents any underlying exception, which caused this exception to be trigger. Usefull in logging scenario.</param>
        /// <param name="httpStatus">Map to an HTTP status in web scenarios.</param>
        public HttpServiceException(string message, Exception inner, HttpStatusCode httpStatus = HttpStatusCode.InternalServerError)
            : base(message, inner)
        {
            HttpStatus = httpStatus;
        }


        public HttpStatusCode HttpStatus { get; private set; }


        public static HttpServiceException New(string message, HttpStatusCode httpStatus, params object[] formatArgs)
        {
            var ret = New(message, null, httpStatus, formatArgs);
            return ret;
        }

        public static HttpServiceException New(string message, Exception? inner, HttpStatusCode httpStatus, params object[] formatArgs)
        {
            var msg = message;

            if (formatArgs.AnyAndNotNull())
            {
                msg = string.Format(message, formatArgs);
            }

            var ret = default(HttpServiceException);


            if (inner != null)
            {
                ret = new HttpServiceException(msg, inner, httpStatus);
            }
            else
            {
                ret = new HttpServiceException(msg, httpStatus);
            }

            return ret;
        }
    }
}

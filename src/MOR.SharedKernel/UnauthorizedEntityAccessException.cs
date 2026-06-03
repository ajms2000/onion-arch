namespace System
{
    // No need to deive from AppException
    public class UnauthorizedEntityAccessException : UnauthorizedAccessException
    {
        public UnauthorizedEntityAccessException()
            : base() { }

        public UnauthorizedEntityAccessException(string message)
            : base(message) { }

        public UnauthorizedEntityAccessException(string message, Exception innerException)
            : base(message, innerException) { }


        public static UnauthorizedEntityAccessException New(string messageTemplate, params object[] args)
        {
            return New(default(Exception), messageTemplate, args);
        }

        public static UnauthorizedEntityAccessException New(Exception? innerException, string messageTemplate, params object[] args)
        {
            var msg = string.Format(messageTemplate, args);

            if (innerException != null)
            {
                return new UnauthorizedEntityAccessException(msg, innerException);
            }

            return new UnauthorizedEntityAccessException(msg);
        }
    }
}

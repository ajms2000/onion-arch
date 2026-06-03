namespace System
{
    public class InvalidParameterException : AppException
    {
        public InvalidParameterException()
            : base("Invalid parameter.") { }
        public InvalidParameterException(string message)
            : base(message) { }
        public InvalidParameterException(string message, Exception innerException)
            : base(message, innerException) { }


        public static InvalidParameterException New(string messageTemplate, params object[] args)
        {
            return New(default(Exception), messageTemplate, args);
        }

        public static InvalidParameterException New(Exception? innerException, string messageTemplate, params object[] args)
        {
            var msg = string.Format(messageTemplate, args);

            if (innerException != null)
            {
                return new InvalidParameterException(msg, innerException);
            }

            return new InvalidParameterException(msg);
        }
    }
}

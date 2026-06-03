namespace System
{
    public class ConflictException : AppException
    {
        public ConflictException()
            : base("A conflict occurred.") { }
        public ConflictException(string message)
            : base(message) { }
        public ConflictException(string message, Exception innerException)
            : base(message, innerException) { }


        public static ConflictException New(string messageTemplate, params object[] args)
        {
            return New(default(Exception), messageTemplate, args);
        }

        public static ConflictException New(Exception? innerException, string messageTemplate, params object[] args)
        {
            var msg = string.Format(messageTemplate, args);

            if (innerException != null)
            {
                return new ConflictException(msg, innerException);
            }

            return new ConflictException(msg);
        }
    }
}

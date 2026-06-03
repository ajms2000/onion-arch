namespace System
{
    public class AppException : ApplicationException
    {
        public AppException()
            : base("Application exception.") { }
        public AppException(string message)
            : base(message, null) { }
        public AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

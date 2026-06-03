namespace System
{
    public abstract class NotFoundException : AppException
    {
        public NotFoundException()
            : base() { }
        public NotFoundException(string message)
            : base(message) { }
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class EntityNotFoundException : NotFoundException
    {
        public EntityNotFoundException()
            : base() { }
        public EntityNotFoundException(string message)
            : base(message) { }
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }


        public static EntityNotFoundException New(string messageTemplate, params object[] args)
        {
            return New(default(Exception), messageTemplate, args);
        }

        public static EntityNotFoundException New(Exception? innerException, string messageTemplate, params object[] args)
        {
            var msg = string.Format(messageTemplate, args);

            if (innerException != null)
            {
                return new EntityNotFoundException(msg, innerException);
            }

            return new EntityNotFoundException(msg);
        }
    }
}

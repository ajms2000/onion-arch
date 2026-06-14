using MOR.Services;

namespace MOR.Exceptions
{
    public class MjrExceptionBuilder<TException>
        where TException : MjrException
    {
        private static readonly System.Reflection.BindingFlags CREATION_FLAGS =
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance;

        private MjrExceptionCategory ErrorCategory;
        private string ErrorCode = MjrCodes.DefaultError;
        private string[]? CustomMessages;
        private Exception? ErrorInner;
        private object[]? MessageFormatParams;


        private MjrExceptionBuilder() { }


        public MjrExceptionBuilder<TException> Category(MjrExceptionCategory category)
        {
            ErrorCategory = category;
            return this;
        }

        public MjrExceptionBuilder<TException> Code(string errorCode)
        {
            ErrorCode = errorCode;
            return this;
        }

        public MjrExceptionBuilder<TException> Message(string errorMessage)
        {
            CustomMessages = errorMessage.WrapInArray();
            return this;
        }

        public MjrExceptionBuilder<TException> Messages(IEnumerable<string> errorMessages)
        {
            CustomMessages = errorMessages.AnyAndNotNull() ? errorMessages.ToArray() : null;
            return this;
        }

        public MjrExceptionBuilder<TException> InnerException(Exception innerException)
        {
            ErrorInner = innerException;
            return this;
        }

        public MjrExceptionBuilder<TException> Format(params object[] messageFormatParams)
        {
            MessageFormatParams = messageFormatParams;
            return this;
        }

        public void Throw()
        {
            var ex = Build();
            throw ex;
        }


        public TException Build(ICodeMessageProvider? messageProvider = null)
        {
            var argList = new List<object?> { ErrorCategory, ErrorCode };

            if (ErrorInner != null)
            {
                argList.Add(ErrorInner);
            }


            var ret = (TException?)Activator.CreateInstance(typeof(TException), CREATION_FLAGS, binder: null, args: argList.ToArray(), culture: null);

            if (ret != null)
            {
                ret.FinalizeMessages(messageProvider, CustomMessages, MessageFormatParams);

                return ret;
            }

            throw new InvalidOperationException("Unable to create exception object.");
        }


        public static MjrExceptionBuilder<TException> New(MjrExceptionCategory category, string errorCode)
        {
            var ret = new MjrExceptionBuilder<TException>()
                .Category(category)
                .Code(errorCode);
            return ret;
        }
    }
}
